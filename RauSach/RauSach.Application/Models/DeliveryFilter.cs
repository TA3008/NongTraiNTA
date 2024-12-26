using RauSach.Core.FrameworkModels;
using RauSach.Core.ValueObjects;

namespace RauSach.Application.Models
{
    public class DeliveryFilter : FilterModel
    {
        public new int limit { get; set; } = 1000;
        public DeliveryState? State { get; set; }
    }
}
