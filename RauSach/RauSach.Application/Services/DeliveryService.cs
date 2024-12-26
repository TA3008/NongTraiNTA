using Microsoft.Extensions.Logging;
using Rausach.Common.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Core.Models;
using RauSach.Core.ValueObjects;

namespace RauSach.Application.Services
{
    public class DeliveryService : IDeliveryService
    {
        public static string DeliveryExistMsg = "Bạn đã có đơn vận chuyển rau vào ngày";
        private readonly ILogger<DeliveryService> _logger;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMailService _mailService;
        private readonly ISystemParameters _systemParameters;

        public DeliveryService(ILogger<DeliveryService> logger,
            IDeliveryRepository deliveryRepository,
            IOrderRepository orderRepository,
            IMailService mailService,
            ISystemParameters systemParameters)
        {
            _logger = logger;
            _deliveryRepository = deliveryRepository;
            _orderRepository = orderRepository;
            _mailService = mailService;
            _systemParameters = systemParameters;
        }

        public async Task ChangeStatusAsync(Guid deliveryId, DeliveryStatus status)
        {
            var delivery = await _deliveryRepository.GetAsync(deliveryId);
            delivery.Status = status;
            delivery.Modified = DateTimeExtensions.UTCNowVN;
            await _deliveryRepository.UpdateAsync(delivery);

            foreach (var item in delivery.Vegetables)
            {
                await _orderRepository.UpdateVegetableDeliveryStatus(delivery.OrderId, item.Id, status);
            }
        }

        public async Task BuildDeliveryAsync()
        {
            var orders = await _orderRepository.FindAsync(new Models.OrderFilter { OrderStatus = OrderStatus.Active });
            _logger.LogError(new Exception("Test"), $"LogError Number of active orders to check: {orders.Count}");
            _logger.LogDebug($"LogDebug Number of active orders to check: {orders.Count}");
            foreach (var order in orders)
            {
                //_mailService.CanDelivery(order, new Vegetable() { Name = "Rau fuck" });
                try
                {
                    foreach (var vegetable in order.Combo.Vegetables.Where(m => m.Delivery != null && m.Delivery.Status != null))
                    {
                        await CheckNotifyAdminAsync(order, vegetable);
                        await CheckNotifyCustomerAsync(order, vegetable);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"BuildDeliveryAsync failed, order Id {order.Id}", e);
                }
            }
        }

        private async Task CheckNotifyAdminAsync(Order order, Vegetable vegetable)
        {
            if (vegetable.Delivery.Status.VegetableDeliveryState != VegetableDeliveryState.Planting) return;
            if (vegetable.Delivery.StartDate.AddDays(vegetable.LifeDay - 1) > DateTimeExtensions.UTCNowVN) return;

            _logger.LogDebug($"Admin notified, order Id: {order.Id}, vegetable {vegetable.Name}-{vegetable.Id}");
            var status = vegetable.Delivery.Status;
            status.VegetableDeliveryState = VegetableDeliveryState.AdminNotified;
            await _orderRepository.UpdateVegetableDeliveryStatus(order.Id, vegetable.Id, status);

            // Gửi mail thông báo cho quản lý vườn rau đã có thể thu hoạch
            _mailService.GardenPlantingUpToDate(order, vegetable);
        }

        // Nếu là rau có thể thu hoạch nhiều lần và đã vận chuyển thì cho vận chuyển liên tục
        private async Task CheckNotifyCustomerAsync(Order order, Vegetable vegetable)
        {
            // Trường hợp đã giao hàng thành công thi notify cho khách hàng là có thể vận chuyển được
            if (vegetable.Delivery.Status.VegetableDeliveryState != VegetableDeliveryState.Succeeded) return;
            if (!vegetable.CanHarvestManyTimes) return;

            _logger.LogDebug($"Customer notified, order Id: {order.Id}, vegetable {vegetable.Name}-{vegetable.Id}");
            var status = vegetable.Delivery.Status;
            status.VegetableDeliveryState = VegetableDeliveryState.CustomerNotified;
            await _orderRepository.UpdateVegetableDeliveryStatus(order.Id, vegetable.Id, status);

            // Gửi mail thông báo cho khách hàng có thể vận chuyển rau
            _mailService.CustomerCanDelivery(order, vegetable);
        }


        public string? Validate(Guid orderId, DateTime? scheduleDate)
        {
            var delivery = GetPendingOrDeliveringDelivery(orderId);

            if (delivery != null && ((DateTimeExtensions.UTCNowVN - delivery.Created).TotalHours > _systemParameters.TimeCanUpdateDeliveryDate || delivery.Status.DeliveryState == DeliveryState.Delivering))
                return DeliveryExistMsg + $" {delivery.Created.ToString("dd/MM/yyyy")}.";

            if (scheduleDate.HasValue)
            {
                var dtUTCNow = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                if (scheduleDate.Value < dtUTCNow || scheduleDate.Value.Subtract(dtUTCNow).TotalDays < _systemParameters.DeliveryGapDays)
                {
                    return $"Ngày vận chuyển phải sau ngày hiện tại ít nhất {_systemParameters.DeliveryGapDays} ngày.";
                }
            }
            return null;
        }

        public string? ValidateAndCreate(Guid orderId, string username, DateTime scheduleDate, List<DeliveryItem> items)
        {
            var error = Validate(orderId, scheduleDate);
            if (error != null) return error;
            var order = _orderRepository.Get(orderId);
            foreach (var deliveryItem in items)
            {
                var vegetable = order.Combo.Vegetables.FirstOrDefault(m => m.Id == deliveryItem.Id);
                if (vegetable == null) continue;
                deliveryItem.Name = vegetable.Name;
                deliveryItem.Id = vegetable.Id;

                var vdelivery = vegetable.Delivery ?? new VegetableDelivery();
                vdelivery.Status.VegetableDeliveryState = VegetableDeliveryState.Requested;
                vdelivery.DeliveryDate = scheduleDate;
                vdelivery.DeliveryDateUpdateTime = DateTimeExtensions.UTCNowVN;
                vdelivery.Weight = deliveryItem.Weight;

                _orderRepository.UpdateVegetableDelivery(orderId, deliveryItem.Id, vdelivery).GetAwaiter().GetResult();
            }

            var delivery = GetPendingOrDeliveringDelivery(orderId);
            delivery = new Delivery
            {
                Id = delivery == null ? Guid.Empty : delivery.Id,
                OrderId = orderId,
                OrderCode = order.Code,
                ScheduleDate = scheduleDate,
                Created = DateTimeExtensions.UTCNowVN,
                CreatedBy = username,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                CustomerPhone = order.CustomerPhone,
                UserName = username,
                Status = new DeliveryStatus { DeliveryState = DeliveryState.Pendding, UpdatedTime = DateTimeExtensions.UTCNowVN },
                Vegetables = items,
                Weight = items.Sum(m => m.Weight)
            };
            _deliveryRepository.UpsertAsync(delivery);
            return null;
        }

        public List<Delivery> GetDeliveries(Guid orderId, DeliveryState deliveryState)
        {
            var deliveries = _deliveryRepository.Find(x => x.OrderId == orderId && x.Status.DeliveryState == deliveryState && x.Deleted != true).ToList();
            return deliveries;
        }

        public Delivery? GetPendingOrDeliveringDelivery(Guid orderId) => _deliveryRepository.Find(m => m.OrderId == orderId && m.Status != null && (m.Status.DeliveryState == DeliveryState.Pendding || m.Status.DeliveryState == DeliveryState.Delivering)).FirstOrDefault();
    }
}
