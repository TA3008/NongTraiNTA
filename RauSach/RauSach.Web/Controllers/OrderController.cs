using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Application.Services;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Core.Repositories;
using RauSach.Core.ValueObjects;
using RauSach.Web.Models;

namespace RauSach.Web.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGardenRepository _gardenRepository;
        private readonly IVegetableRepository _vegetableRepository;
        private readonly IVegetableComboRepository _vegetableComboRepository;
        private readonly IOrderService _orderService;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IFileService _fileService;
        private readonly ISystemParameters _systemParameters;
        private readonly IDeliveryService _deliveryService;
        private readonly IMailService _mailService;

        public OrderController(ILogger<HomeController> logger,
            IGardenRepository gardenRepository,
            IVegetableRepository vegetableRepository,
            IVegetableComboRepository vegetableComboRepository,
            IOrderService orderService,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IVoucherRepository voucherRepository,
            IFileService fileService,
            ISystemParameters systemParameters,
            IDeliveryService deliveryService,
            IMailService mailService)
        {
            _logger = logger;
            _gardenRepository = gardenRepository;
            _vegetableRepository = vegetableRepository;
            _vegetableComboRepository = vegetableComboRepository;
            _orderService = orderService;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _fileService = fileService;
            _systemParameters = systemParameters;
            _voucherRepository = voucherRepository;
            _deliveryService = deliveryService;
            _mailService = mailService;
        }

        public IActionResult Index()
        {
            var order = new OrderViewModel();
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = _userRepository.GetByUsername(User.Identity.Name);
                order = new OrderViewModel()
                {
                    CustomerAddress = user.Address,
                    CustomerName = user.Name,
                    CustomerPhone = user.PhoneNumber
                };
            }
            var bankInfo = _systemParameters.BankInfo;
            if (!string.IsNullOrEmpty(bankInfo))
            {
                bankInfo = bankInfo.Replace("\r\n", "<br/>");
            }
            ViewData["BankInfo"] = bankInfo ?? "";
            return View(order);
        }

        [HttpGet]
        public IActionResult GetCombos(Guid gardenId)
        {
            return ViewComponent("OrderComboList", new { gardenId = gardenId });
        }

        [HttpGet]
        public IActionResult GetCombo(Guid comboId)
        {
            if (comboId == Guid.Empty)
            {
                var vegetables = _vegetableRepository.GetAll();
                return PartialView("_OrderCombo", new VegetableCombo()
                {
                    Vegetables = vegetables,
                    Name = "Tuỳ chọn",
                    CanCustomize = true
                });
            }
            var combo = _vegetableComboRepository.Get(comboId);
            return PartialView("_OrderCombo", combo);
        }

        [HttpPost]
        public IActionResult Order(OrderViewModel model, List<VegetableViewModel> vegetables)
        {
            if (!ModelState.IsValid)
            {
                string messages = base.GetModalStateErrorMsg();
                return Json(new JsonReturn(false, messages));
            }

            if (User.Identity?.IsAuthenticated != true)
            {
                return Json(new JsonReturn(false, "Vui lòng đăng nhập để tạo đơn hàng."));
            }

            if (model.GardenArea <= 0)
            {
                return Json(new JsonReturn(false, "Vui lòng chọn diện tích vườn."));
            }

            var garden = _gardenRepository.Get(model.GardenId);
            if (garden == null)
            {
                return Json(new JsonReturn(false, "Có lỗi xảy ra với vườn đã chọn, vui lòng refresh trình duyệt và chọn lại vườn!"));
            }

            if ((vegetables == null || vegetables.Any() == false) && model.ComboId == Guid.Empty)
            {
                return Json(new JsonReturn(false, $"Chưa chọn vườn hoặc chưa chọn loại rau trồng!"));
            }

            if (model.PaymentType != PaymentType.SixMonths.GetHashCode()
                && model.PaymentType != PaymentType.TwelveMonths.GetHashCode()
                && model.PaymentType != PaymentType.Month.GetHashCode())
            {
                return Json(new JsonReturn(false, $"Chưa chọn hình thức thanh toán!"));
            }
            //6 tháng giảm 10%, 12 tháng giảm 20%
            long price = 0;
            if (model.PaymentType == PaymentType.SixMonths.GetHashCode())
            {
                price = garden.Price * model.GardenArea * 6 * (100 - 10) / 100;
            }
            else if (model.PaymentType == PaymentType.TwelveMonths.GetHashCode())
            {
                price = garden.Price * model.GardenArea * 12 * (100 - 20) / 100;
            }

            var order = new Order();
            order.PaymentType = (PaymentType)model.PaymentType;
            if (!String.IsNullOrEmpty(model.VoucherCode))
            {
                var voucher = _voucherRepository.Find(x => x.Code == model.VoucherCode).FirstOrDefault();
                if (voucher == null || voucher.StartDate.Date > DateTimeExtensions.UTCNowVN.Date)
                {
                    return Json(new JsonReturn(false, $"Mã giảm giá {model.VoucherCode} không tồn tại!"));
                }
                if (voucher.Expired.Date < DateTimeExtensions.UTCNowVN.Date)
                {
                    return Json(new JsonReturn(false, $"Mã giảm giá {model.VoucherCode} đã hết hạn!"));
                }
                if (voucher.Quantity <= 0)
                {
                    return Json(new JsonReturn(false, $"Mã giảm giá {model.VoucherCode} đã hết!"));
                }
                if (voucher.DiscountRate > 0)
                {
                    price = price * (100 - voucher.DiscountRate) / 100;
                }
                else if (voucher.DiscountAmount > 0)
                {
                    price -= voucher.DiscountAmount;
                }
                order.Voucher = voucher;
                var quantity = voucher.Quantity - 1 > 0 ? voucher.Quantity - 1 : 0;
                _voucherRepository.UpdateQuantity(voucher.Id, quantity);
            }

            order.Created = DateTimeExtensions.UTCNowVN;
            order.CreatedBy = User?.Identity?.Name;
            order.ModifiedBy = User?.Identity?.Name;
            order.Modified = DateTimeExtensions.UTCNowVN;

            garden.Area = model.GardenArea;
            order.Garden = garden;
            if (model.ComboId != Guid.Empty)
            {
                order.Combo = _vegetableComboRepository.Get(model.ComboId);
            }
            else
            {
                var vegs = _vegetableRepository.GetAll();
                var modelVegetables = vegetables;
                vegs = vegs.Where(x => modelVegetables.Select(y => y.Id).Contains(x.Id)).ToList();
                vegs.ForEach(x =>
                {
                    x.Area = modelVegetables.First(y => y.Id == x.Id).Area;
                });
                order.Combo.Vegetables = vegs;
                order.Combo.Name = "Tự chọn loại rau";
            }

            order.Price = price + (garden.Price * model.GardenArea * 3); //đặt cọc trước 3 tháng
            order.Status = OrderStatus.Pendding;
            order.StatusHistories.Add(new OrderStatusDetails { ActionTime = DateTimeExtensions.UTCNowVN, Status = order.Status, Author = User?.Identity?.Name });
            order.Username = User.Identity.Name;
            order.CustomerAddress = model.CustomerAddress;
            order.CustomerName = model.CustomerName;
            order.CustomerPhone = model.CustomerPhone;
            order.CustomerNote = model.CustomerNote;
            order.ImageUrl = model.ImageUrl;
            order.Code = User.Identity.Name.Length > 4 ? User.Identity.Name.Substring(0, 4) + DateTimeExtensions.UTCNowVN.ToString("yyMMddHHmmss")
                : User.Identity.Name.Length + DateTimeExtensions.UTCNowVN.ToString("yyMMddHHmmss");


            _orderRepository.UpsertAsync(order);
            _mailService.OrderStatusChanged(order);

            return Json(new JsonReturn(true, "Đặt hàng thành công!"));
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                // var date = DateTimeExtensions.UTCNowVN;
                // string ext = Path.GetExtension(file.FileName);
                // var url = _fileService.UpsertImage("orders", $"{date.Year}/{date.Month}/{Guid.NewGuid()}.{date.ToString("yyyyMMdd")}.{ext ?? "png"}", file.OpenReadStream());
                // Lấy phần mở rộng của file
                string extension = Path.GetExtension(file.FileName);
                var date = DateTimeExtensions.UTCNowVN;
                string extensions = Path.GetExtension(file.FileName)?.TrimStart('.') ?? "jpg"; // Loại bỏ dấu chấm nếu có
                string imageName = $"{date.Year}-{date.Month}-{Guid.NewGuid()}-{date:yyyyMMddHHmmss}.{extensions}";
                string url = imageName;
                var result = await _fileService.UploadFile(file, @"orders", imageName);
                if (result == null)
                {
                    return Json(new JsonReturn(false, "Unsupported file type"));
                }
                return Json(new JsonReturn(true, url));
            }
            return Json(new JsonReturn(false));
        }

        public IActionResult MyOrder()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = _orderService.GetOrdersByUsername(User.Identity.Name).OrderByDescending(x => x.Created).ToList();
            return View(orders);
        }

        public IActionResult MyGardens()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = _orderService.GetActivePendingPaidOrdersByUsername(User.Identity.Name);
            return View(orders);
        }

        public IActionResult MyGarden(string code)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewData["GapDays"] = _systemParameters.DeliveryGapDays;
            var order = _orderRepository.Find(x => x.Code == code && x.Deleted != true).FirstOrDefault();

            List<HarvestedVegetableViewModel> lsHarvested = new List<HarvestedVegetableViewModel>();
            var deliveries = _deliveryService.GetDeliveries(order.Id, DeliveryState.Succeeded);
            if (deliveries.Any())
            {

                order.Combo.Vegetables.ForEach(x =>
                {
                    var harvestedGr = 0;
                    deliveries.ForEach(y =>
                    {
                        var h = y.Vegetables.FirstOrDefault(z => z.Id == x.Id);
                        if (h != null)
                        {
                            harvestedGr += h.Weight;
                        }
                    });
                    lsHarvested.Add(new HarvestedVegetableViewModel()
                    {
                        Id = x.Id,
                        HarvestedGr = harvestedGr
                    });
                });
            }
            ViewData["Harvested"] = lsHarvested;
            return View(order);
        }

        /// <summary>
        /// Lưu ngày vận chuyển rau
        /// date, month, year theo UTC
        /// </summary>
        /// <param name="vegId"></param>
        /// <param name="date"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveDeliveryTime(Guid orderId, int date, int month, int year, List<DeliveryItem> vegetables)
        {
            if (orderId == Guid.Empty)
            {
                return Json(new JsonReturn(false, "Chưa chọn vườn của tôi. Refresh trình duyệt để update trạng thái mới."));
            }
            if (vegetables?.Any() != true)
            {
                return Json(new JsonReturn(false, "Chưa chọn loại rau vận chuyển."));
            }
            if (vegetables.Any(x => x.Weight <= 0) == true)
            {
                return Json(new JsonReturn(false, "Khối lượng vận chuyển của rau phải lớn hơn 0."));
            }

            var order = _orderRepository.Get(orderId);
            if (order == null || order.Status != OrderStatus.Active)
            {
                return Json(new JsonReturn(false, "Không có rau nào đến ngày thu hoạch hoặc vườn đang được thiết lập, hết hạn."));
            }

            var error = _deliveryService.Validate(orderId, scheduleDate: null);
            if (error != null)
            {
                return Json(new JsonReturn(false, error));
            }

            var vegs = order.Combo.Vegetables.Where(x => x.HarvestProceedLife == 100
                                                    && (x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.CustomerNotified
                                                    || x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.Requested
                                                    || x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.Succeeded
                                                    || x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.Failed)).ToList();

            foreach (var vegModel in vegetables)
            {
                var veg = vegs.FirstOrDefault(x => x.Id == vegModel.Id);
                if (vegModel.Weight > veg.GramPerM2 * veg.Area)
                {
                    return Json(new JsonReturn(false, $"{veg.Name} vượt quá khối lượng thu hoạch tối đa {veg.GramPerM2 * veg.Area} (g)."));
                }
            }

            if (date <= 0 || month <= 0 | year <= 0)
            {
                return Json(new JsonReturn(false, "Chưa chọn ngày vận chuyển"));
            }

            var garden = _gardenRepository.Get(order.Garden.Id);
            if (garden.DeliveryWeight < vegetables.Select(x => x.Weight).Sum())
            {
                return Json(new JsonReturn(false, $"Vượt quá khối lượng rau tối đa vận chuyển một lần của {garden.Name} là {garden.DeliveryWeight} g."));
            }

            DateTime scheduleDate = new DateTime(year, month, date);
            scheduleDate = scheduleDate.AddHours(7);
            var errorMessage = _deliveryService.ValidateAndCreate(orderId, User.Identity.Name, scheduleDate, vegetables);

            if (errorMessage != null) return Json(new JsonReturn(false, errorMessage));

            return Json(new JsonReturn(true));
        }

        public IActionResult MyVegetablesCanDelivery(Guid orderId)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }
            var order = _orderRepository.Get(orderId);
            if (order == null || order.Status != OrderStatus.Active)
            {
                TempData["Error"] = "Không có rau nào đến ngày thu hoạch hoặc vườn đang được thiết lập, hết hạn.";
                return RedirectToAction("Error", "Home");
            }

            var error = _deliveryService.Validate(orderId, scheduleDate: null);
            if (error != null)
            {
                if (error.Contains(DeliveryService.DeliveryExistMsg))
                {
                    var delivery = _deliveryService.GetPendingOrDeliveringDelivery(orderId);
                    ViewData["Delivery"] = delivery;
                }
                ViewData["Error"] = error;
                return PartialView("_VegetablesCanDelivery");
            }

            var vegs = order.Combo.Vegetables.Where(x => x.HarvestProceedLife == 100
                                                    && (x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.CustomerNotified
                                                    || x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.Requested
                                                    || x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.Succeeded
                                                    || x.Delivery.Status.VegetableDeliveryState == VegetableDeliveryState.Failed)).ToList();

            return PartialView("_VegetablesCanDelivery", vegs);
        }

        public IActionResult Voucher(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                return Json(new JsonReturn(false, $"Mã giảm giá trống!"));
            }
            code = code.Trim();
            var voucher = _voucherRepository.Find(x => x.Code == code).FirstOrDefault();
            if (voucher == null
                || new DateTime(voucher.StartDate.Year, voucher.StartDate.Month, voucher.StartDate.Day) > new DateTime(DateTimeExtensions.UTCNowVN.Year, DateTimeExtensions.UTCNowVN.Month, DateTimeExtensions.UTCNowVN.Day))
            {
                return Json(new JsonReturn(false, $"Mã giảm giá {code} không tồn tại!"));
            }
            if (voucher.Expired.Date < DateTimeExtensions.UTCNowVN.Date)
            {
                return Json(new JsonReturn(false, $"Mã giảm giá {code} đã hết hạn!"));
            }
            if (voucher.Quantity <= 0)
            {
                return Json(new JsonReturn(false, $"Mã giảm giá {code} đã hết!"));
            }
            return Json(new JsonReturn(true, voucher.DiscountRate > 0 ? $"{voucher.DiscountRate}%" : $"{voucher.DiscountAmount}vnd"));
        }
    }
}
