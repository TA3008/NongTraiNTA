using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Services;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Web.Filters;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [WebAuthorize(RoleList.Admin)]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ISystemParameters _systemParameters;
        private readonly IDeliveryService _deliveryService;
        private readonly IMailService _mailService;

        public AdminController(ILogger<AccountController> logger,
            ISystemParameters systemParameters,
            IDeliveryService deliveryService,
            IMailService mailService
            )
        {
            _logger = logger;
            _systemParameters = systemParameters;
            _deliveryService = deliveryService;
            _mailService = mailService;
        }

        public IActionResult SystemParameters()
        {
            return View(_systemParameters.GetValues());
        }

        public IActionResult EditSystemParameter(string name)
        {
            var value = _systemParameters.GetValue(name);
            var data = PropertyExtensions.GetDataTypes<ISystemParameters>().FirstOrDefault(m => m.DataName == name);
            var model = new SystemParamData { DataName = name, DataValue = value, Type = data?.Type, Description = data?.Description };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSystemParameter(SystemParamData model, string dataValue)
        {
            _systemParameters.SetValue(model.DataName, dataValue);
            return RedirectToAction(nameof(SystemParameters));
        }

        [HttpGet]
        public async Task<IActionResult> SchedulerJobs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteJobs()
        {
            await _deliveryService.BuildDeliveryAsync();
            ViewBag.Success = $"{DateTimeExtensions.UTCNowVN}: Test chạy job thành công";
            return View(viewName: nameof(SchedulerJobs));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendTestMail(string toAddress, string subject, string body)
        {
            try
            {
                _mailService.Send(toAddress, subject, body);
                ViewBag.Success = $"{DateTimeExtensions.UTCNowVN}: Test gửi email thành công";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"{DateTimeExtensions.UTCNowVN}: Test gửi email thất bại. {ex.Message}";
                _logger.LogError(ex, "Send mail failed");
            }
            return View(viewName: nameof(SchedulerJobs));
        }
    }
}
