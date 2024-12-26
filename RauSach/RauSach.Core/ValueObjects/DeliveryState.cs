using System.ComponentModel;

namespace RauSach.Core.ValueObjects
{
    public enum DeliveryState
    {
        [Description("Chờ vận chuyển")]
        Pendding = 0,

        [Description("Đang vận chuyển")]
        Delivering = 10,

        [Description("Vận chuyển thành công")]
        Succeeded = 20,

        [Description("Vận chuyển thất bại")]
        Failed = 30
    }
}
