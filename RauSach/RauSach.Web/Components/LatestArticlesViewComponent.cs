using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;

namespace RauSach.Web.Components
{
    public class LatestArticlesViewComponent : ViewComponent
    {
        private readonly IArticleRepository _articleRepository;

        public LatestArticlesViewComponent(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public IViewComponentResult Invoke()
        {
            var numberOfArticles = 3;
            var articles = _articleRepository.Find(x => x.IsPublished && x.Deleted == false).OrderByDescending(x => x.Created).Take(numberOfArticles).ToList();

            return View(articles);
        }
    }
}
