using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Models;
using RauSach.Application.Repositories;
using RauSach.Application.Services;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Core.ValueObjects;
using RauSach.Web.Filters;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [WebAuthorize(RoleList.Delivery, RoleList.Admin)]
    public class DeliveryController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IDeliveryService _deliveryService;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        private readonly IOrderRepository _orderRepository;
        public DeliveryController(ILogger<OrderController> logger,
            IDeliveryService deliveryService,
            IDeliveryRepository deliveryRepository,
            IFileService fileService,
            IMailService mailService,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _deliveryService = deliveryService;
            _deliveryRepository = deliveryRepository;
            _fileService = fileService;
            _mailService = mailService;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index(DeliveryFilter filter)
        {
            if (filter == null) filter = new DeliveryFilter { State = DeliveryState.Delivering };

            ViewBag.SearchModel = filter;
            var deliveries = await _deliveryRepository.FindAsync(filter);
            return View(deliveries);
        }

        public async Task<IActionResult> Details(Guid id, string returnUrl)
        {
            var model = await _deliveryRepository.GetAsync(id);
            model.Order = await _orderRepository.GetAsync(model.OrderId) ?? new Order();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDelivery(Delivery delivery, IFormFile fileInput, string actionType, string returnUrl)
        {
            var model = await _deliveryRepository.GetAsync(delivery.Id);
            // if (fileInput != null)
            // {
            //     var date = DateTimeExtensions.UTCNowVN;
            //     var url = _fileService.UpsertImage("deliveries", $"{date.Year}/{date.Month}/{model.Id}.png", fileInput.OpenReadStream());
            //     await _deliveryRepository.SetAsync(delivery.Id, nameof(Delivery.ImageUrl), url);
            // }
            if (fileInput != null)
            {
                string extension = Path.GetExtension(fileInput.FileName);
                var date = DateTimeExtensions.UTCNowVN;
                string imageName = $"{date.Year}/{date.Month}/{Guid.NewGuid()}.{date:yyyyMMdd}.{extension ?? "png"}";
                await _fileService.UploadFile(fileInput, "deliveries", imageName);
            }
            await _deliveryRepository.SetAsync(delivery.Id, m => m.Note, delivery.Note);
            var deliveryStatus = new DeliveryStatus
            {
                DeliveryState = DeliveryState.Delivering,
                VegetableDeliveryState = VegetableDeliveryState.Delivering,
                UpdatedTime = DateTimeExtensions.UTCNowVN
            };
            if (actionType == "succeeded")
            {
                deliveryStatus.DeliveryState = DeliveryState.Succeeded;
                deliveryStatus.VegetableDeliveryState = VegetableDeliveryState.Succeeded;
            }
            else if (actionType == "failed")
            {
                deliveryStatus.DeliveryState = DeliveryState.Failed;
                deliveryStatus.VegetableDeliveryState = VegetableDeliveryState.Failed;
            }
            await _deliveryService.ChangeStatusAsync(delivery.Id, deliveryStatus);

            delivery = await _deliveryRepository.GetAsync(delivery.Id);
            _mailService.CustomerDeliveryAction(delivery);

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> Delete(Guid id, string returnUrl)
        {
            await _deliveryRepository.SetAsync(id, nameof(Delivery.Deleted), true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }
    }
}
