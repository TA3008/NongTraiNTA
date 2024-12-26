using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Infrastructures;
using RauSach.Application.Repositories;
using RauSach.Core.FrameworkModels;
using RauSach.Web.Filters;
using RauSach.Web.Models;

namespace RauSach.Web.Controllers
{
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

        public IActionResult List()
        {
            var model = _articleRepository.Find(x => x.IsPublished && x.Deleted == false).OrderByDescending(x => x.Created).ToList();
            return View(model);
        }

        public IActionResult Detail(string url)
        {
            var article = _articleRepository.Find(x => x.FriendlyUrl == url && x.Deleted == false && x.IsPublished).ToList().FirstOrDefault();
            return View(article);
        }

        [WebAuthorize(RoleList.Content, RoleList.Admin)]
        public IActionResult Preview(string url)
        {
            var article = _articleRepository.Find(x => x.FriendlyUrl == url && x.Deleted == false).ToList().FirstOrDefault();
            return View("Detail", article);
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file != null)
            {
                var url = _fileService.UpsertImage("imgs", $"{DateTimeExtensions.UTCNowVN.Year}/{DateTimeExtensions.UTCNowVN.Month}/{Guid.NewGuid()}.png", file.OpenReadStream());
                return Json(new { location = url });
            }
            return Json("Không upload được ảnh!");
        }
    }
}
