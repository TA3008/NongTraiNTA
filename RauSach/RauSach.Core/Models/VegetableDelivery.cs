using RauSach.Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace RauSach.Core.Models
{
    public class VegetableDelivery
    {
        // Ngày bắt đầu gieo trồng
        [Required(ErrorMessage = "Ngày gieo trồng không được trống")]
        [Display(Name = "Ngày gieo trồng")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày khách chọn vận chuyển
        /// </summary>
        [Display(Name = "Ngày vận chuyển")]
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Thời gian cập nhật ngày vận chuyển
        /// </summary>
        public DateTime DeliveryDateUpdateTime { get; set; }
        public DeliveryStatus Status { get; set; } = new DeliveryStatus();
        public bool IsCustomerRequestedDelivery { get; set; }
        public string? DeliveryRequestedBy { get; set; }

        public int Weight { get; set; }
    }
}
