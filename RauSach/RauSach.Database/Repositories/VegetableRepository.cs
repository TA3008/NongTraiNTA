using MongoDB.Driver;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Database.Repositories
{
    public class VegetableRepository : BaseRepository<Vegetable>, IVegetableRepository
    {
        public VegetableRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
