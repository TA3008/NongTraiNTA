using System.ComponentModel;

namespace RauSach.Core.ValueObjects
{
    public enum PaymentType
    {
        [Description("6 tháng giảm 10%")]
        SixMonths = 1,

        [Description("12 tháng giảm 20%")]
        TwelveMonths = 2,

        [Description("Thanh toán hàng tháng")]
        Month = 3
    }
}
