using RauSach.Application.Models;
using RauSach.Core.Models;
using RauSach.Core.Repositories;

namespace RauSach.Application.Repositories
{
    public interface IDeliveryRepository : IBaseRepository<Delivery>
    {
        Task<List<Delivery>> FindAsync(DeliveryFilter filter);
    }
}
