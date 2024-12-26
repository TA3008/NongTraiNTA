using MongoDB.Driver;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Database.Repositories
{
    public class VegetableComboRepository : BaseRepository<VegetableCombo>, IVegetableComboRepository
    {
        public VegetableComboRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
