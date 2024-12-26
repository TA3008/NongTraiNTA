using MongoDB.Driver;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Database.Repositories
{
    public class ArticleRepository : BaseRepository<Article>, IArticleRepository
    {
        public ArticleRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
