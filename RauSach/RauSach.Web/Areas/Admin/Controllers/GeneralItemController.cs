using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Application.Services;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Web.Filters;
using RauSach.Web.Models;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [WebAuthorize("Quản lý Nông dân", RoleList.Sale, RoleList.Admin)]
    public class GeneralItemController : Controller
    {
        private readonly ILogger<GeneralItemController> _logger;
        private readonly IGeneralItemRepository _generalItemRepository;
        private readonly IFileService _fileService;
        private readonly IOrderService _orderService;

        public GeneralItemController(ILogger<GeneralItemController> logger,
            IGeneralItemRepository generalItemRepository,
            IFileService fileService,
            IOrderService orderService)
        {
            _logger = logger;
            _generalItemRepository = generalItemRepository;
            _fileService = fileService;
            _orderService = orderService;
        }

        public IActionResult Farmers()
        {
            var gardens = _generalItemRepository.GetAll().Where(m => !m.Deleted).OrderByDescending(m => m.Created).ToList();
            return View(gardens);
        }

        public IActionResult EditFarmer(Guid? id)
        {
            GeneralItem? model = null;
            if (id.HasValue)
            {
                model = _generalItemRepository.Get(id.Value);
                var orders = _orderService.GetByFarmer(id.Value);
                if (orders?.Any() == true)
                {
                    var generalItemGarden = orders.GroupBy(x => x.Garden.Id).Select(g =>
                    {
                        var os = g.ToList();
                        return new GeneralItemGardenViewModel()
                        {
                            FarmerId = id.Value,
                            GardenName = os.First().Garden.Name,
                            GardenCodes = os.Select(x => x.GardenCode).ToList()
                        };
                    }).ToList();
                    ViewData["GeneralItemGarden"] = generalItemGarden;
                }
            }
            if (model == null)
            {
                model = new GeneralItem { Created = DateTime.UtcNow.AddHours(7) };
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> EditFarmer(GeneralItem model, IFormFile fileInput, string returnUrl) => Submit(model, fileInput, "farmers", returnUrl);

        public IActionResult Sales()
        {
            var items = _generalItemRepository.GetAll().Where(m => !m.Deleted && m.Type == "Sale").OrderByDescending(m => m.Created).ToList();
            return View(items);
        }

        public IActionResult EditSale(Guid? id)
        {
            GeneralItem? model = null;
            if (id.HasValue)
            {
                model = _generalItemRepository.Get(id.Value);
            }
            if (model == null)
            {
                model = new GeneralItem { Created = DateTime.UtcNow.AddHours(7), Type = "Sale" };
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> EditSale(GeneralItem model, IFormFile fileInput, string returnUrl) => Submit(model, fileInput, "sales", returnUrl);

        public async Task<IActionResult> Delete(Guid id, string returnUrl)
        {
            await _generalItemRepository.SetAsync(id, m => m.Deleted, true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Farmers));
            else return Redirect(returnUrl);
        }

        private async Task<IActionResult> Submit(GeneralItem model, IFormFile fileInput, string rootFolder, string returnUrl)
        {
            model.ModifiedBy = User?.Identity?.Name;
            model.Modified = DateTime.UtcNow.AddHours(7);
            if (model.Id == Guid.Empty)
            {
                model.CreatedBy = model.ModifiedBy;
            }
            model = await _generalItemRepository.UpdateAsync(model);
            if (fileInput != null)
            {
                string ext = Path.GetExtension(fileInput.FileName);
                var url = _fileService.UpsertImage(rootFolder, $"{model.Id}/{Guid.NewGuid()}{ext ?? ".png"}", fileInput.OpenReadStream());
                model.ImageUrl = url;
                model.Thumbnail = model.ImageUrl;
                model = await _generalItemRepository.UpdateAsync(model);
            }

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(rootFolder);
            else return Redirect(returnUrl);
        }
    }
}
