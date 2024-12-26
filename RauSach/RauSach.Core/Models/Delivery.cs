using Rausach.Common.Extensions;
using RauSach.Core.ValueObjects;

namespace RauSach.Core.Models
{
    public class Delivery : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public string? OrderCode { get; set; }
        public string? UserName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
        public string? Note { get; set; }
        public DateTime ScheduleDate { get; set; }
        public List<DeliveryItem> Vegetables { get; set; } = new List<DeliveryItem>();
        public string? ImageUrl { get; set; }
        public int Weight { get; set; }
        public DeliveryStatus Status { get; set; } = new DeliveryStatus();

        public static IEnumerable<(DeliveryState value, string? text)> GetPossibleStatuses()
        {
            var values = Enum.GetValues(typeof(DeliveryState)).Cast<DeliveryState>();
            foreach (var item in values)
            {
                yield return new(item, item.GetEnumDescription());
            }
        }
    }
}
