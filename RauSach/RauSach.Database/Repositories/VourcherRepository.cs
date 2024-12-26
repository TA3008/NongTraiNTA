using MongoDB.Driver;
using RauSach.Application.Repositories;
using RauSach.Core.Models;
using RauSach.Core.ValueObjects;

namespace RauSach.Database.Repositories
{
    public class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(IMongoDatabase db) : base(db)
        {
        }

        public async Task UpdateQuantity(Guid id, int quantity)
        {
            var filter = Builders<Voucher>.Filter.Eq(x => x.Id, id);

            var update = Builders<Voucher>.Update.Set(x => x.Quantity, quantity);

            var result = await _collection.FindOneAndUpdateAsync(filter, update,
                options: new FindOneAndUpdateOptions<Voucher> { ReturnDocument = ReturnDocument.After }
            );
        }
    }
}
