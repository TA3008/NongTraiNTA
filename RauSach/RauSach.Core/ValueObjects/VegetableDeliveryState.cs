using System.ComponentModel;

namespace RauSach.Core.ValueObjects
{
    public enum VegetableDeliveryState
    {
        [Description("Đang gieo trồng")]
        Planting = 0,

        [Description("Đã báo quản lý vườn")]
        AdminNotified = 10,

        [Description("Đã báo khách hàng")]
        CustomerNotified = 20,

        [Description("Đã yêu cầu vận chuyển")]
        Requested = 30,

        [Description("Đang vận chuyển")]
        Delivering = 40,

        [Description("Đã vận chuyển")]
        Succeeded = 50,

        [Description("Vận chuyển thất bại")]
        Failed = 60
    }
}
