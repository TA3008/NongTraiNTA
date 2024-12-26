using RauSach.Core.Models;
using RauSach.Core.Repositories;

namespace RauSach.Application.Repositories
{
    public interface IVoucherRepository : IBaseRepository<Voucher>
    {
        Task UpdateQuantity(Guid id, int quantity);
    }
}
