using System.ComponentModel.DataAnnotations;

namespace RauSach.Core.Models
{
    public class Voucher : BaseEntity
    {
        [Display(Name = "Mã Voucher")]
        [Required(ErrorMessage = "Mã không được để trống")]
        public string? Code { get; set; }

        [Display(Name = "Số lượng ban đầu")]
        [Required(ErrorMessage = "Số lượng không được để trống")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn hàng sử dụng")]
        public List<Guid> UsedOrderIds { get; set; } = new List<Guid>();

        [Display(Name = "Phần trăm giảm giá (%)")]
        public int DiscountRate { get; set; }

        [Display(Name = "Giảm giá (vnđ)")]
        public int DiscountAmount { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày hết hạn")]
        [Required(ErrorMessage = "Ngày hết hạn không được để trống")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Expired { get; set; }

        [Display(Name = "Mô tả")]
        public string? Details { get; set; }
    }
}
