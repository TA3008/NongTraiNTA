using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;

namespace RauSach.Web.Components
{
    public class OtherArticlesViewComponent : ViewComponent
    {
        private readonly IArticleRepository _articleRepository;

        public OtherArticlesViewComponent(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public IViewComponentResult Invoke(Guid currentArticleId, DateTime currentArticleCreatedDate)
        {
            var numberOfArticles = 3;
            var articles = _articleRepository.Find(x => x.Id != currentArticleId && x.Created >= currentArticleCreatedDate && x.IsPublished && x.Deleted == false).Take(numberOfArticles).ToList();
            if(articles.Count < numberOfArticles)
            {
                var remainArticles = _articleRepository.Find(x => x.Id != currentArticleId && x.Created < currentArticleCreatedDate && x.IsPublished && x.Deleted == false)
                    .OrderByDescending(y => y.Created).Take(numberOfArticles - articles.Count).ToList();
                articles.AddRange(remainArticles);
            }

            return View(articles);
        }
    }
}
