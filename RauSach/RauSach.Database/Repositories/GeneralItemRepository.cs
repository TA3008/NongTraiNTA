using MongoDB.Driver;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Database.Repositories
{
    public class GeneralItemRepository : BaseRepository<GeneralItem>, IGeneralItemRepository
    {
        public GeneralItemRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
