using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Web.Filters;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [WebAuthorize("Quản lý Voucher", RoleList.Admin)]
    public class VoucherController : Controller
    {
        private readonly ILogger<VoucherController> _logger;
        private readonly IVoucherRepository _voucherRepository;

        public VoucherController(ILogger<VoucherController> logger,
            IVoucherRepository voucherRepository)
        {
            _logger = logger;
            _voucherRepository = voucherRepository;
        }

        public IActionResult Index()
        {
            var gardens = _voucherRepository.GetAll().Where(m => !m.Deleted).OrderByDescending(m => m.Created).ToList();
            return View(gardens);
        }

        public IActionResult Edit(Guid? id)
        {
            Voucher? model = null;
            if (id.HasValue)
            {
                model = _voucherRepository.Get(id.Value);
            }
            if (model == null)
            {
                model = new Voucher { Expired = DateTime.UtcNow.AddHours(7), StartDate = DateTime.UtcNow.AddHours(7) };
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Voucher model, string returnUrl)
        {
            model.ModifiedBy = User?.Identity?.Name;
            model.Modified = DateTime.UtcNow.AddHours(7);
            model.Expired = new DateTime(model.Expired.Year, model.Expired.Month, model.Expired.Day).AddHours(7);
            model.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day).AddHours(7);
            if (model.Id == Guid.Empty)
            {
                model.CreatedBy = model.ModifiedBy;
                model.Created = DateTime.UtcNow.AddHours(7);
            }
            if (model.StartDate > model.Expired)
            {
                ModelState.AddModelError("StartDate", "Ngày bắt đầu phải nhỏ hơn ngày hết hạn.");
                return View(model);
            }
            if(model.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Số lượng ban đầu phải lớn hơn 0.");
                return View(model);
            }
            if (model.DiscountRate > 0 && model.DiscountAmount > 0)
            {
                ModelState.AddModelError("DiscountRate", "Chỉ được chọn giảm giá theo % hoặc giảm giá theo tiền.");
                return View(model);
            }

            await _voucherRepository.UpdateAsync(model);

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> Delete(Guid id, string returnUrl)
        {
            await _voucherRepository.SetAsync(id, nameof(Voucher.Deleted), true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Edit));
            else return Redirect(returnUrl);
        }
    }
}
