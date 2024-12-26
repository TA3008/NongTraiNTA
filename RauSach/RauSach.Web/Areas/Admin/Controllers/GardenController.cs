using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Database.Repositories;
using RauSach.Infrastructure.File;
using RauSach.Web.Filters;
using RauSach.Web.Helpers;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [WebAuthorize(RoleList.Product, RoleList.Admin)]
    public class GardenController : Controller
    {
        private readonly ILogger<GardenController> _logger;
        private readonly IGardenRepository _gardenRepository;
        private readonly IVegetableRepository _vegetableRepository;
        private readonly IVegetableComboRepository _vegetableComboRepository;
        private readonly IFileService _fileService;

        public GardenController(ILogger<GardenController> logger,
            IGardenRepository gardenRepository,
            IVegetableRepository vegetableRepository,
            IVegetableComboRepository vegetableComboRepository,
            IFileService fileService)
        {
            _logger = logger;
            _gardenRepository = gardenRepository;
            _vegetableRepository = vegetableRepository;
            _vegetableComboRepository = vegetableComboRepository;
            _fileService = fileService;
        }

        public IActionResult Gardens()
        {
            var gardens = _gardenRepository.GetAll().Where(m => !m.Deleted).OrderByDescending(m => m.Created).ToList();
            return View(gardens);
        }

        public IActionResult EditGarden(Guid? id)
        {
            Garden? model = null;
            if (id.HasValue)
            {
                model = _gardenRepository.Get(id.Value);
            }
            if (model == null)
            {
                model = new Garden();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGarden(Garden model, IFormFile fileInput, string returnUrl)
        {
            model.ModifiedBy = User?.Identity?.Name;
            model.Modified = DateTime.UtcNow.AddHours(7);
            if (model.Id == Guid.Empty)
            {
                model.CreatedBy = model.ModifiedBy;
            }
            if (String.IsNullOrEmpty(model.FriendlyUrl))
            {
                var url = StringHelpers.ToFriendlyUrl(model.Name);
                if (_gardenRepository.Find(x => x.FriendlyUrl == url && x.Deleted != true).FirstOrDefault() != null)
                {
                    do
                    {
                        model.FriendlyUrl = url + "-" + new Random().Next(1, 100);
                    }
                    while (_gardenRepository.Find(x => x.FriendlyUrl == model.FriendlyUrl && x.Deleted != true).FirstOrDefault() != null);
                }
                else
                {
                    model.FriendlyUrl = url;
                }
            }
            if (fileInput != null)
            {
                string extension = Path.GetExtension(fileInput.FileName);
                string image = StringHelpers.ToFriendlyUrl(model.Name) + extension;
                model.Thumbnail = await _fileService.UploadFile(fileInput, @"garden", image.ToLower());
            }
            if (string.IsNullOrEmpty(model.Thumbnail)) model.Thumbnail = "default.jpg";

            model = await _gardenRepository.UpdateAsync(model);
            // if (fileInput != null)
            // {
            //     string ext = Path.GetExtension(fileInput.FileName);
            //     var url = _fileService.UpsertImage("gardens", $"{model.Id}/{Guid.NewGuid()}{ext ?? ".png"}", fileInput.OpenReadStream());
            //     model.ImageUrl = url;
            //     model.Thumbnail = _fileService.ResizeImageJpeg(fileInput.OpenReadStream(), 360, 230, "gardens", $"{model.Id}/{Guid.NewGuid()}.thumb{ext ?? ".png"}");
                
            //     model = await _gardenRepository.UpdateAsync(model);
            // }
            

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Gardens));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> DeleteGarden(Guid id, string returnUrl)
        {
            await _gardenRepository.SetAsync(id, nameof(Garden.Deleted), true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Gardens));
            else return Redirect(returnUrl);
        }

        public IActionResult Vegetables()
        {
            var model = _vegetableRepository.GetAll().Where(m => !m.Deleted).OrderByDescending(m => m.Created).ToList();
            return View(model);
        }

        public IActionResult EditVegetable(Guid? id)
        {
            Vegetable? model = null;
            if (id.HasValue)
            {
                model = _vegetableRepository.Get(id.Value);
            }
            if (model == null)
            {
                model = new Vegetable();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVegetable(Vegetable model, IFormFile fileInput, string returnUrl)
        {
            model.ModifiedBy = User?.Identity?.Name;
            model.Modified = DateTime.UtcNow.AddHours(7);
            if (model.Id == Guid.Empty)
            {
                model.CreatedBy = model.ModifiedBy;
            }
            if (fileInput != null)
            {
                string extension = Path.GetExtension(fileInput.FileName);
                string image = StringHelpers.ToFriendlyUrl(model.Name) + extension;
                model.Thumbnail = await _fileService.UploadFile(fileInput, @"vegetables", image.ToLower());
            }
            if (string.IsNullOrEmpty(model.Thumbnail)) model.Thumbnail = "default.jpg";
            model = await _vegetableRepository.UpdateAsync(model);
            // if (fileInput != null)
            // {
            //     string ext = Path.GetExtension(fileInput.FileName);
            //     var url = _fileService.UpsertImage("vegetables", $"{model.Id}/{Guid.NewGuid()}{ext ?? ".png"}", fileInput.OpenReadStream());
            //     model.ImageUrl = url;
            //     model.Thumbnail = model.ImageUrl;
            //     model = await _vegetableRepository.UpdateAsync(model);
            // }
            
            var combos = _vegetableComboRepository.GetAll().Where(m => m.Vegetables.Any(m => m.Id == model.Id)).ToList();
            foreach (var combo in combos)
            {
                var index = combo.Vegetables.FindIndex(m => m.Id == model.Id);
                combo.Vegetables[index] = model;
                await _vegetableComboRepository.UpdateAsync(combo);
            }

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Vegetables));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> DeleteVegetable(Guid id, string returnUrl)
        {
            await _vegetableRepository.SetAsync(id, nameof(Vegetable.Deleted), true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Vegetables));
            else return Redirect(returnUrl);
        }

        public IActionResult VegetableCombos()
        {
            ViewBag.Gardens = _gardenRepository.GetAll().Where(m => !m.Deleted).ToList();
            var model = _vegetableComboRepository.GetAll().Where(m => !m.Deleted).OrderBy(m => m.GardenId).ThenByDescending(m => m.Created).ToList();
            return View(model);
        }

        public IActionResult UpdateVegetableCombo(Guid? id)
        {
            VegetableCombo? model = null;
            if (id.HasValue)
            {
                model = _vegetableComboRepository.Get(id.Value);
            }
            if (model == null)
            {
                model = new VegetableCombo();
            }

            ViewBag.Gardens = _gardenRepository.GetAll().Where(m => !m.Deleted).ToList();
            ViewBag.Vegetables = _vegetableRepository.GetAll().Where(m => !m.Deleted).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVegetableCombo(VegetableCombo model, string returnUrl)
        {
            model.ModifiedBy = User?.Identity?.Name;
            model.Modified = DateTime.UtcNow.AddHours(7);
            if (model.Id == Guid.Empty)
            {
                model.CreatedBy = model.ModifiedBy;
            }

            var vegetables = _vegetableRepository.GetAll().Where(m => !m.Deleted).ToList();
            var updatedVegetables = new List<Vegetable>();
            foreach (var item in model.Vegetables.Where(m => m.Id != Guid.Empty))
            {
                var vegetable = vegetables.FirstOrDefault(m => m.Id == item.Id);
                if (vegetable == null) continue;
                vegetable.Area = item.Area;
                updatedVegetables.Add(vegetable);
            }
            model.Vegetables = updatedVegetables;

            await _vegetableComboRepository.UpdateAsync(model);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(VegetableCombos));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> DeleteVegetableCombo(Guid id, string returnUrl)
        {
            await _vegetableComboRepository.SetAsync(id, nameof(VegetableCombo.Deleted), true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(VegetableCombos));
            else return Redirect(returnUrl);
        }
    }
}
