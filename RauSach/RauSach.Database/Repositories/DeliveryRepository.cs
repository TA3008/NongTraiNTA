using MongoDB.Bson;
using MongoDB.Driver;
using RauSach.Application.Models;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Database.Repositories
{
    public class DeliveryRepository : BaseRepository<Delivery>, IDeliveryRepository
    {
        public DeliveryRepository(IMongoDatabase db) : base(db)
        {
        }

        public Task<List<Delivery>> FindAsync(DeliveryFilter filter)
        {
            var builder = Builders<Delivery>.Filter;
            FilterDefinition<Delivery> filterDefinition = Builders<Delivery>.Filter.Empty;

            if (filter.State.HasValue)
                filterDefinition &= builder.Eq(m => m.Status.DeliveryState, filter.State.Value);

            if (!string.IsNullOrWhiteSpace(filter.query))
            {
                filterDefinition &= builder.Or(builder.Regex(m => m.CustomerName, new BsonRegularExpression($"{filter.query}", "i")) |
                    builder.Regex(m => m.CustomerPhone, new BsonRegularExpression($"{filter.query}", "i")));
            }

            return _collection.Find(filterDefinition).Skip(filter.start).Limit(filter.limit).ToListAsync();
        }
    }
}
