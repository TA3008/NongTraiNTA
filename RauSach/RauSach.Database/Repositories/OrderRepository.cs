using MongoDB.Bson;
using MongoDB.Driver;
using Rausach.Common.Extensions;
using RauSach.Application.Models;
using RauSach.Application.Repositories;
using RauSach.Core.Models;
using RauSach.Core.ValueObjects;

namespace RauSach.Database.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(IMongoDatabase db) : base(db)
        {
        }

        public Task<List<Order>> FindAsync(OrderFilter filter)
        {
            var builder = Builders<Order>.Filter;
            FilterDefinition<Order> filterDefinition = Builders<Order>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(filter.Code))
                filterDefinition &= builder.Eq(m => m.Code, filter.Code.Trim());

            if (filter.OrderStatus.HasValue)
                filterDefinition &= builder.Eq(m => m.Status, filter.OrderStatus.Value);

            if (filter.VegetableDeliveryState.HasValue)
                filterDefinition &= builder.ElemMatch(m => m.Combo.Vegetables, x => x.Delivery != null && x.Delivery.Status.VegetableDeliveryState == filter.VegetableDeliveryState);

            if (!string.IsNullOrWhiteSpace(filter.query))
            {
                filterDefinition &= builder.Or(builder.Regex(m => m.CustomerName, new BsonRegularExpression($"{filter.query.Trim()}", "i")) |
                    builder.Regex(m => m.CustomerPhone, new BsonRegularExpression($"{filter.query.Trim()}", "i")));
            }

            if (!string.IsNullOrWhiteSpace(filter.Garden))
            {
                filterDefinition &= builder.Or(builder.Regex(m => m.GardenCode, new BsonRegularExpression($"{filter.Garden.Trim()}", "i")) |
                    builder.Regex(m => m.Garden.Name, new BsonRegularExpression($"{filter.Garden.Trim()}", "i")));
            }

            if (filter.CreatedFrom.HasValue)
                filterDefinition &= builder.Gte(m => m.Created, new DateTime(filter.CreatedFrom.Value.Year, filter.CreatedFrom.Value.Month, filter.CreatedFrom.Value.Day));

            if (filter.CreatedTo.HasValue)
                filterDefinition &= builder.Lte(m => m.Created, new DateTime(filter.CreatedTo.Value.Year, filter.CreatedTo.Value.Month, filter.CreatedTo.Value.Day, 23, 59, 59));

            return _collection.Find(filterDefinition).Skip(filter.start).Limit(filter.limit).ToListAsync();
        }

        public async Task UpdateVegetableDeliveryStatus(Guid id, Guid vegetableId, DeliveryStatus status)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.Id, id)
            & Builders<Order>.Filter.ElemMatch(x => x.Combo.Vegetables, Builders<Vegetable>.Filter.Eq(x => x.Id, vegetableId));

            var update = Builders<Order>.Update.Set(x => x.Combo.Vegetables[-1].Delivery.Status, status);

            var result = await _collection.FindOneAndUpdateAsync(filter, update,
                options: new FindOneAndUpdateOptions<Order> { ReturnDocument = ReturnDocument.After }
            );
        }

        public async Task UpdateVegetableDelivery(Guid id, Guid vegetableId, VegetableDelivery vegetableDelivery)
        {
            var filter = Builders<Order>.Filter.Eq(x => x.Id, id)
            & Builders<Order>.Filter.ElemMatch(x => x.Combo.Vegetables, Builders<Vegetable>.Filter.Eq(x => x.Id, vegetableId));

            var update = Builders<Order>.Update.Set(x => x.Combo.Vegetables[-1].Delivery, vegetableDelivery);

            var result = await _collection.FindOneAndUpdateAsync(filter, update,
                options: new FindOneAndUpdateOptions<Order> { ReturnDocument = ReturnDocument.After }
            );
        }
    }
}
