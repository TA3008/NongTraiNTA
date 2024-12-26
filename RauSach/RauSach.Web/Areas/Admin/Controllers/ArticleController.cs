using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Models;
using RauSach.Infrastructure.File;
using RauSach.Web.Filters;
using RauSach.Web.Helpers;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [WebAuthorize(RoleList.Content, RoleList.Admin)]
    public class ArticleController : Controller
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleRepository _articleRepository;
        private readonly IFileService _fileService;

        public ArticleController(ILogger<ArticleController> logger,
            IArticleRepository articleRepository,
            IFileService fileService)
        {
            _logger = logger;
            _articleRepository = articleRepository;
            _fileService = fileService;

        }

        public IActionResult Index()
        {
            var articles = _articleRepository.Find(x => x.Deleted == false).ToList();
            return View(articles);
        }

        public IActionResult Edit(Guid? id)
        {
            Article? model = null;
            if (id.HasValue)
            {
                model = _articleRepository.Get(id.Value);
            }
            if (model == null)
            {
                model = new Article();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Article model, IFormFile fileInput, string returnUrl)
        {
            if (!ModelState.IsValid && (!ModelState.ContainsKey("returnUrl") && !ModelState.ContainsKey("fileInput")))
            {
                return View(model);
            }
            model.ModifiedBy = User?.Identity?.Name;
            model.Modified = DateTimeExtensions.UTCNowVN;
            if (model.Id == Guid.Empty)
            {
                model.CreatedBy = model.ModifiedBy;
                model.Created = DateTimeExtensions.UTCNowVN;
            }
            // if (fileInput != null)
            // {
            //     var url = _fileService.UpsertImage("gardens", $"{model.Id}.png", fileInput.OpenReadStream());
            //     model.Img = url;
            //     model.Thumbnail = _fileService.ResizeImageJpeg(fileInput.OpenReadStream(), 360, 230, "gardens", $"{model.Id}.thumb.png");
            // }
            if (fileInput != null)
            {
                string extension = Path.GetExtension(fileInput.FileName);
                string image = StringHelpers.ToFriendlyUrl(model.Name) + extension;
                model.Img = image.ToLower();
                model.Thumbnail = await _fileService.UploadFile(fileInput, @"posts", image.ToLower());
            }
            if (string.IsNullOrEmpty(model.Thumbnail)) model.Thumbnail = "default.jpg";
            if (String.IsNullOrEmpty(model.FriendlyUrl))
            {
                var url = StringHelpers.ToFriendlyUrl(model.Name);
                if (_articleRepository.Find(x => x.FriendlyUrl == url && x.Deleted != true).FirstOrDefault() != null)
                {
                    do
                    {
                        model.FriendlyUrl = url + "-" + new Random().Next(1, 100);
                    }
                    while (_articleRepository.Find(x => x.FriendlyUrl == model.FriendlyUrl && x.Deleted != true).FirstOrDefault() != null);
                }
                else
                {
                    model.FriendlyUrl = url;
                }
            }
            await _articleRepository.UpdateAsync(model);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }

        public async Task<IActionResult> Delete(Guid id, string returnUrl)
        {
            await _articleRepository.SetAsync(id, nameof(Article.Deleted), true);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Index));
            else return Redirect(returnUrl);
        }
    }
}
