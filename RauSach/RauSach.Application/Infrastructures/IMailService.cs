using RauSach.Core.Models;

namespace RauSach.Application.Infrastructures
{
    public interface IMailService
    {
        /// <summary>
        /// Gửi thông báo rau đến ngày thu hoạch cho quản lí vườn
        /// </summary>
        /// <param name="recievers"></param>
        public void GardenPlantingUpToDate(Order order, Vegetable vegetable);

        /// <summary>
        /// Gửi thông báo có thể thu hoạch rau cho khách hàng
        /// </summary>
        /// <param name="order"></param>
        /// <param name="vegetable"></param>
        public void CustomerCanDelivery(Order order, Vegetable vegetable);
        void CustomerDeliveryAction(Delivery delivery);
        void OrderStatusChanged(Order order);

        void Send(string to, string subject, string body);
    }
}
