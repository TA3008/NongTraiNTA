using RauSach.Core.Models;
using System.ComponentModel;

namespace RauSach.Application.Services
{
    public interface ISystemParameters
    {
        object GetValue(string name);
        void SetValue(string name, object value);
        List<SystemParamData> GetValues();

        [Description("Thông tin ngân hàng")]
        public string? BankInfo { get; }

        [Description("Số ngày có thể vận chuyển sau ngày hiện tại")]
        public int DeliveryGapDays { get; }

        /// <summary>
        /// (giờ) Quá thời gian này thì ko cho phép đổi ngày vận chuyển rau nữa
        /// </summary>
        [Description("Thời gian có thể thay đổi ngày vận chuyển rau (giờ)")]
        public int TimeCanUpdateDeliveryDate { get; }

        [Description("Giới hạn số đơn vận chuyển rau một ngày")]
        public int LimitDeliveryADay { get; }

        [Description("SMPT email host")]
        public string? SmtpHost { get; }

        [Description("SMPT email port")]
        public int SmtpPort { get; }

        [Description("SMPT email")]
        public string? SmtpEmail { get; }

        [Description("SMPT email password")]
        public string? SmtpPassword { get; }

        [Description("Email kế toán (Mỗi email một dòng)")]
        public string? AccountingEmails { get; }

        [Description("Email quản lý vườn (Mỗi email một dòng)")]
        public string? GardenManagerEmails { get; }

        [Description("Tên miền")]
        public string? Domain { get; }
    }
}
