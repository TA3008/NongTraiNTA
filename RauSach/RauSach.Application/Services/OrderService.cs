using RauSach.Application.Repositories;
using RauSach.Core.Models;
using RauSach.Core.ValueObjects;

namespace RauSach.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetOrdersByUsername(string username)
        {
            return _orderRepository.Find(x => x.Username == username).ToList();
        }

        public List<Order> GetActiveOrdersByUsername(string username)
        {
            return _orderRepository.Find(x => x.Username == username && x.Status == OrderStatus.Active).ToList();
        }

        public List<Order> GetActivePendingPaidOrdersByUsername(string username)
        {
            return _orderRepository.Find(x => x.Username == username && (x.Status == OrderStatus.Active 
            || x.Status == OrderStatus.Pendding
            || x.Status == OrderStatus.Paid)).ToList();
        }

        public List<Order> GetByFarmer(Guid farmerId)
        {
            return _orderRepository.Find(x => x.FarmerId == farmerId).ToList();
        }
    }
}
