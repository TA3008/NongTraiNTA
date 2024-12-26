using RauSach.Core.Models;
using RauSach.Core.ValueObjects;

namespace RauSach.Application.Services
{
    public interface IDeliveryService
    {
        Task ChangeStatusAsync(Guid deliveryId, DeliveryStatus status);

        /// <summary>
        /// Tính toán rau đến ngày thu hoạch/vận chuyển
        /// </summary>
        /// <returns></returns>
        Task BuildDeliveryAsync();
        string? Validate(Guid orderId, DateTime? scheduleDate);
        string? ValidateAndCreate(Guid orderId, string username, DateTime scheduleDate, List<DeliveryItem> items);
        List<Delivery> GetDeliveries(Guid orderId, DeliveryState deliveryState);
        Delivery? GetPendingOrDeliveringDelivery(Guid orderId);
    }
}
