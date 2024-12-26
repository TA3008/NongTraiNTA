using RauSach.Application.Models;
using RauSach.Core.Models;
using RauSach.Core.Repositories;
using RauSach.Core.ValueObjects;

namespace RauSach.Application.Repositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<Order>> FindAsync(OrderFilter filter);
        Task UpdateVegetableDeliveryStatus(Guid id, Guid vegetableId, DeliveryStatus status);
        Task UpdateVegetableDelivery(Guid id, Guid vegetableId, VegetableDelivery vegetableDelivery);
    }
}
