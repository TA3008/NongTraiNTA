using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Models;
using RauSach.Application.Repositories;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Core.ValueObjects;
using RauSach.Web.Filters;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [WebAuthorize(RoleList.Account, RoleList.Admin, RoleList.OrderManager)]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IVegetableRepository _vegetableRepository;
        private readonly IVegetableComboRepository _vegetableComboRepository;
        private readonly IGardenRepository _gardenRepository;
        private readonly IMailService _mailService;
        private readonly IGeneralItemRepository _generalItemRepository;

        public OrderController(ILogger<OrderController> logger,
            IOrderRepository orderRepository,
            IVegetableRepository vegetableRepository,
            IVegetableComboRepository vegetableComboRepository,
            IGardenRepository gardenRepository,
            IMailService mailService,
            IGeneralItemRepository generalItemRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _vegetableRepository = vegetableRepository;
            _vegetableComboRepository = vegetableComboRepository;
            _gardenRepository = gardenRepository;
            _mailService = mailService;
            _generalItemRepository = generalItemRepository;
        }

        public async Task<IActionResult> Index(OrderFilter filter)
        {
            if (filter == null) filter = new OrderFilter { OrderStatus = OrderStatus.Pendding };

            ViewBag.SearchModel = filter;
            var model = await _orderRepository.FindAsync(filter);
            return View(model);
        }

        public IActionResult EditByGardenCode(string gardenCode)
        {
            var order = _orderRepository.Find(x => x.GardenCode == gardenCode).FirstOrDefault();
            if(order != null)
            {
                return RedirectToAction("Edit", new { id = order.Id });
            }
            return View("NotExist", gardenCode);
        }

        public IActionResult Edit(Guid id)
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];
            var model = _orderRepository.Get(id);

            var garden = _gardenRepository.Get(model.Garden.Id);
            if (garden == null) ViewBag.GardenItems = new List<string>();
            else
            {
                var gardenItems = garden.GetGardenItems().Where(m => m.Area == model.Garden.Area);
                var gardenCodesUsed = _orderRepository.Find(m => m.Id != model.Id && m.Garden.Id == garden.Id).Select(m => m.GardenCode).Distinct().ToList();
                ViewBag.GardenItems = gardenItems.Where(m => !gardenCodesUsed.Contains(m.Code)).Select(m => m.Code).OrderBy(m => m).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order model, string returnUrl)
        {
            var order = await _orderRepository.GetAsync(model.Id);
            order.ModifiedBy = User?.Identity?.Name;
            order.Modified = DateTimeExtensions.UTCNowVN;
            order.GardenCode = model.GardenCode;
            order.CustomerAddress = model.CustomerAddress;
            order.CustomerNote = model.CustomerNote;
            order.SaleId = model.SaleId;

            if (model.FarmerId.HasValue)
            {
                order.FarmerId = model.FarmerId;
                var farmer = await _generalItemRepository.GetAsync(model.FarmerId.Value);
                order.FarmerName = farmer?.Title;
            }
            foreach (var item in order.Combo.Vegetables)
            {
                var vege = model.Combo.Vegetables.FirstOrDefault(m => m.Id == item.Id);
                item.Area = vege == null ? item.Area : vege.Area;
            }

            await _orderRepository.UpdateAsync(order);
            TempData["Success"] = $"{DateTimeExtensions.UTCNowVN.ToString("dd/MM/yyyy hh:mm:ss")}: Cập nhật đơn hàng thành công";
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> UpdateStatus(Guid id, OrderStatus status, string returnUrl)
        {
            var order = await _orderRepository.GetAsync(id);
            order.ModifiedBy = User?.Identity?.Name;
            order.Modified = DateTimeExtensions.UTCNowVN;
            order.Status = status;
            order.StatusHistories.Add(new OrderStatusDetails { ActionTime = DateTimeExtensions.UTCNowVN, Status = status, Author = User?.Identity?.Name });

            if (status == OrderStatus.Active)
            {
                foreach (var item in order.Combo.Vegetables)
                {
                    item.Delivery = item.Delivery ?? new VegetableDelivery();
                    item.Delivery.StartDate = DateTimeExtensions.UTCNowVN;
                }
            }
            await _orderRepository.UpdateAsync(order);

            _logger.LogDebug($"Status updated to {status}, order Id: {order.Id}");
            _mailService.OrderStatusChanged(order);

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> Delete(Guid id, string returnUrl)
        {
            await _orderRepository.SetAsync(id, nameof(Order.Deleted), true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }
    }
}
