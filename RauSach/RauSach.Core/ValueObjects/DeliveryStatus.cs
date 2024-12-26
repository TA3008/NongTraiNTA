using Rausach.Common.Extensions;

namespace RauSach.Core.ValueObjects
{
    public class DeliveryStatus
    {
        public VegetableDeliveryState VegetableDeliveryState { get; set; }
        public DeliveryState DeliveryState { get; set; }
        public string Message { get; set; }
        public DateTime UpdatedTime { get; set; }

        public static IEnumerable<(VegetableDeliveryState value, string text)> GetVegetableStatuses()
        {
            var values = Enum.GetValues(typeof(VegetableDeliveryState)).Cast<VegetableDeliveryState>();
            foreach (var item in values)
            {
                yield return new(item, item.GetEnumDescription());
            }
        }
    }
}
