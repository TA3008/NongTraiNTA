using RauSach.Core.FrameworkModels;
using RauSach.Core.ValueObjects;

namespace RauSach.Application.Models
{
    public class OrderFilter : FilterModel
    {
        public new int limit { get; set; } = 200;
        public OrderStatus? OrderStatus { get; set; }
        public VegetableDeliveryState? VegetableDeliveryState { get; set; }
        public string? Code { get; set; }
        public string? Garden { get; set; }

        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
