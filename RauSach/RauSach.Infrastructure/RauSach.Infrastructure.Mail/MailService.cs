using Microsoft.Extensions.Logging;
using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Application.Services;
using RauSach.Core.Models;
using RauSach.Core.Repositories;
using RauSach.Core.Services;
using RauSach.Core.ValueObjects;
using System.Net;
using System.Net.Mail;

namespace RauSach.Infrastructure.Mail
{
    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly ISystemParameters _systemParameters;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IGeneralItemRepository _generalItemRepository;
        public MailService(ILogger<MailService> logger, ISystemParameters systemParameters, IEmailTemplate emailTemplate, IUserRepository userRepository, IOrderRepository orderRepository, IGeneralItemRepository generalItemRepository)
        {
            _logger = logger;
            _systemParameters = systemParameters;
            _emailTemplate = emailTemplate;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _generalItemRepository = generalItemRepository;
        }

        public void CustomerCanDelivery(Order order, Vegetable vegetable)
        {
            var emails = GetUserEmails(order);
            if (emails != null)
            {
                var body = _emailTemplate.GetTemplate("CustomerCanDelivery.html");
                body = body.Replace("__Link__", $"{_systemParameters.Domain}/vuon-cua-toi");
                body = body.Replace("__VegetableName__", vegetable.Name);
                Send(emails, $"{vegetable.Name} đã sẵn sàng thu hoạch!", body);
                _logger.LogDebug($"Email sent CustomerCanDelivery, order Id: {order.Id}, vegetable {vegetable.Name}-{vegetable.Id}");
            }
        }

        public void CustomerDeliveryAction(Delivery delivery)
        {
            var order = _orderRepository.Get(delivery.OrderId);
            var emails = GetUserEmails(order);
            if (order != null && emails != null)
            {
                var body = _emailTemplate.GetTemplate($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}.html");
                body = body.Replace("__Link__", $"{_systemParameters.Domain}/vuon-cua-toi");
                body = body.Replace("__OrderCode__", order.Code);
                body = body.Replace("__Date__", delivery.ScheduleDate.ToString("dd/MM/yyyy"));
                body = body.Replace("__Weight__", delivery.Weight.ToString("N0"));
                var vegetables = delivery.Vegetables.Select(x => $"{x.Name} - {x.Weight} g");
                body = body.Replace("__Vegetables__", string.Join("<br />", vegetables));
                var title = "Rau của bạn đang được vận chuyển";
                if (delivery?.Status?.DeliveryState == DeliveryState.Succeeded)
                {
                    title = "Rau của bạn được vận chuyển thành công";
                }
                else if (delivery?.Status?.DeliveryState == DeliveryState.Failed)
                {
                    title = "Rau của bạn được vận chuyển thất bại";
                }
                var success = delivery?.Status?.DeliveryState == DeliveryState.Succeeded ? "thành công" : "thất bại";
                Send(emails, title, body);
                _logger.LogDebug($"Email sent {System.Reflection.MethodBase.GetCurrentMethod()?.Name}, order Id: {order.Id}, delivery Id: {delivery.Id}");
            }
        }

        public void GardenPlantingUpToDate(Order order, Vegetable vegetable)
        {
            if (!string.IsNullOrWhiteSpace(_systemParameters.GardenManagerEmails))
            {
                var body = _emailTemplate.GetTemplate("GardenPlantingUpToDate.html");
                body = body.Replace("__Link__", $"{_systemParameters.Domain}/admin/planting/Edit?orderId={order.Id}&vegetableId={vegetable.Id}");
                body = body.Replace("__VegetableName__", vegetable.Name);
                body = body.Replace("__OrderCode__", order.Code);
                body = body.Replace("__GardenCode__", order.GardenCode);
                body = body.Replace("__FarmerName__", order.FarmerName);
                Send(_systemParameters.GardenManagerEmails, $"{vegetable.Name} đã đến ngày thu hoạch", body);
                _logger.LogDebug($"Email sent GardenPlantingUpToDate, order Id: {order.Id}, vegetable {vegetable.Name}-{vegetable.Id}");
            }
        }

        public void OrderStatusChanged(Order order)
        {
            switch (order.Status)
            {
                case OrderStatus.Pendding:
                    SendCustomerOrder(order, "CustomerOrderPendding", $"{_systemParameters.Domain}/don-hang-cua-toi", "Đơn hàng của bạn đang chờ duyệt");

                    // Gửi mail cho kế toán
                    AccountantOrderPending(order);
                    break;
                case OrderStatus.Paid:
                    SendCustomerOrder(order, "CustomerOrderApproved", $"{_systemParameters.Domain}/vuon-cua-toi", "Đơn hàng của bạn đã được duyệt");

                    // Gửi mail cho quản lý vườn
                    GardenOrderApproved(order);
                    break;
                case OrderStatus.Active:
                    SendCustomerOrder(order, "CustomerOrderActivated", $"{_systemParameters.Domain}/vuon-cua-toi", "Vườn rau của bạn đã bắt đầu gieo trồng");
                    break;
                case OrderStatus.Canceled:
                    SendCustomerOrder(order, "CustomerOrderCanceled", $"{_systemParameters.Domain}/don-hang-cua-toi", "Đơn hàng của bạn đã bị hủy");
                    break;
                default:
                    break;

            }
        }

        private void AccountantOrderPending(Order order)
        {
            if (!string.IsNullOrWhiteSpace(_systemParameters.AccountingEmails))
            {
                var body = _emailTemplate.GetTemplate("AccountantOrderPending.html");
                body = body.Replace("__Link__", $"{_systemParameters.Domain}/admin/order/edit/{order.Id}");
                body = body.Replace("__OrderCode__", order.Code);
                Send(_systemParameters.AccountingEmails, $"Đơn hàng cần duyệt {order.Code}", body);
                _logger.LogDebug($"Email sent accounting notified, order Id: {order.Id}");
            }
        }

        private void GardenOrderApproved(Order order)
        {
            if (!string.IsNullOrWhiteSpace(_systemParameters.GardenManagerEmails))
            {
                var body = _emailTemplate.GetTemplate("GardenOrderApproved.html");
                body = body.Replace("__Link__", $"{_systemParameters.Domain}/admin/order/edit/{order.Id}");
                body = body.Replace("__OrderCode__", order.Code);
                Send(_systemParameters.GardenManagerEmails, $"Đơn hàng {order.Code} đã được duyệt", body);
                _logger.LogDebug($"Email sent GardenOrderApproved, order Id: {order.Id}");
            }
        }

        private void SendCustomerOrder(Order order, string templateName, string link, string title)
        {
            var emails = GetUserEmails(order);
            if (emails != null)
            {
                var body = _emailTemplate.GetTemplate($"{templateName}.html");
                body = body.Replace("__Link__", link);
                body = body.Replace("__OrderCode__", order.Code);
                Send(emails, title, body);
                _logger.LogDebug($"Email sent SendCustomerOrder, order Id: {order.Id}, status: {order.Status}");
            }
        }

        public void Send(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                _logger.LogDebug($"Recipients is empty, subject: {subject}");
                return;
            }
            var fromAddress = new MailAddress(_systemParameters.SmtpEmail, "Nông trại NTA");

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = _systemParameters.SmtpPort,// Gmail = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_systemParameters.SmtpEmail, _systemParameters.SmtpPassword)
            };
            using (var message = new MailMessage()
            {
                Subject = $"Nông trại NTA - {subject}",
                Body = body
            })
            {
                message.From = fromAddress;
                foreach (var address in to.Split(new[] { ';', '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }
                message.IsBodyHtml = true;
                smtp.Send(message);
            }

            _logger.LogDebug($"Email sent to: {to}");
        }

        private string? GetUserEmails(Order order)
        {
            if (order == null) return null;
            var user = _userRepository.GetByUsername(order.Username);
            var sale = order.SaleId.HasValue ? _generalItemRepository.Get(order.SaleId.Value) : null;
            var emails = $"{user.Email};{sale?.Email}";
            return emails.Length > 1 ? emails : null;
        }
    }
}
