using System.ComponentModel;

namespace RauSach.Core.ValueObjects
{
    public enum OrderStatus
    {
        [Description("Khởi tạo")]
        Initial = 0,

        [Description("Chờ duyệt")]
        Pendding = 10,

        [Description("Đã hủy")]
        Canceled = 20,

        [Description("Đã duyệt")]
        Paid = 30,

        [Description("Đang hoạt động")]
        Active = 40,

        [Description("Đã hết hạn")]
        Expired = 50
    }
}
