using Microsoft.AspNetCore.Mvc.Rendering;
using RauSach.Core.Models;
using RauSach.Core.ValueObjects;
using RauSach.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace RauSach.Web.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Chưa chọn vườn")]
        public Guid GardenId { get; set; }

        [Required(ErrorMessage = "Chưa chọn diện tích vườn")]
        public int GardenArea { get; set; }

        public Guid ComboId { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? CustomerAddress { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression("(84|0[3|5|7|8|9]|0\\d{2})+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? CustomerPhone { get; set; }
        public string? CustomerNote { get; set; }

        [Required(ErrorMessage = "Ảnh chụp màn hình thanh toán không được để trống")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Mã voucher
        /// </summary>
        public string? VoucherCode { get; set; }

        /// <summary>
        /// Hình thức thanh toán
        /// </summary>
        [Required(ErrorMessage = "Phải chọn hình thức thanh toán")]
        [Range(1, 3, ErrorMessage = "Phải chọn hình thức thanh toán")]
        public int PaymentType { get; set; }

        public IEnumerable<SelectListItem> PaymentTypes
        {
            get
            {
                return EnumHelper.EnumToListItems<PaymentType>();
            }
        }
    }

    public class VegetableViewModel
    {
        public Guid Id { get; set; }

        public int Area { get; set; }

    }
}
