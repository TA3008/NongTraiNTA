using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Models;
using RauSach.Application.Repositories;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Web.Filters;
using RauSach.Web.Models;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [WebAuthorize(RoleList.Planting, RoleList.Admin)]
    public class PlantingController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IMailService _mailService;
        public PlantingController(ILogger<OrderController> logger,
            IOrderRepository orderRepository,
            IMailService mailService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _mailService = mailService;
        }

        public async Task<IActionResult> Index(OrderFilter filter)
        {
            if (filter == null) filter = new OrderFilter { OrderStatus = Core.ValueObjects.OrderStatus.Active };

            ViewBag.SearchModel = filter;
            var orders = await _orderRepository.FindAsync(filter);
            var model = new List<PlantingModel>();
            foreach (var order in orders)
            {
                if (filter.VegetableDeliveryState.HasValue)
                {
                    order.Combo.Vegetables = order.Combo.Vegetables.Where(m => m.Delivery?.Status?.VegetableDeliveryState == filter.VegetableDeliveryState.Value).ToList();
                }
                var planting =
                        new PlantingModel
                        {
                            Order = order
                        };
                model.Add(planting);
            }
            return View(model);
        }

        public IActionResult Edit(Guid orderId, Guid vegetableId)
        {
            var order = _orderRepository.Get(orderId);
            var vegetable = order.Combo.Vegetables.FirstOrDefault(m => m.Id == vegetableId);
            var model = new PlantingModel { StartDate = vegetable.Delivery.StartDate, Order = order, Vegetable = vegetable };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PlantingModel model, string returnUrl)
        {
            var order = _orderRepository.Get(model.Order.Id);
            var vegetable = order.Combo.Vegetables.FirstOrDefault(m => m.Id == model.Vegetable.Id);
            if (vegetable != null)
            {
                var delivery = vegetable.Delivery ?? new VegetableDelivery();
                if (model.StartPlanting)
                {
                    delivery.Status.VegetableDeliveryState = Core.ValueObjects.VegetableDeliveryState.Planting;
                }
                delivery.StartDate = model.StartDate.AddHours(7);

                await _orderRepository.UpdateVegetableDelivery(model.Order.Id, model.Vegetable.Id, delivery);
            }
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> UpdateVegetableStatus(Guid orderId, Guid vegetableId, string returnUrl)
        {
            var order = _orderRepository.Get(orderId);
            var vegetable = order.Combo.Vegetables.FirstOrDefault(m => m.Id == vegetableId);
            if (vegetable != null)
            {
                var delivery = vegetable.Delivery ?? new VegetableDelivery();
                delivery.Status.VegetableDeliveryState = Core.ValueObjects.VegetableDeliveryState.CustomerNotified;
                delivery.DeliveryDateUpdateTime = DateTimeExtensions.UTCNowVN;
                _mailService.CustomerCanDelivery(order, vegetable);

                await _orderRepository.UpdateVegetableDelivery(orderId, vegetableId, delivery);
            }

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }
    }
}
