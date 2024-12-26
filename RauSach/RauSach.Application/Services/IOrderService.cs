using RauSach.Core.Models;

namespace RauSach.Application.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Lấy ra các đơn hàng của khách hàng
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        List<Order> GetOrdersByUsername(string username);

        /// <summary>
        /// Những đơn hàng đang active
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        List<Order> GetActiveOrdersByUsername(string username);

        /// <summary>
        /// những đơn đang active hoặc pending
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>

        List<Order> GetActivePendingPaidOrdersByUsername(string username);

        List<Order> GetByFarmer(Guid farmerId);
    }
}
