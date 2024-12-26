using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RauSach.Core.Models
{
    public class Contact : BaseEntity
    {
        [Display(Name = "Chủ đề")]
        [Required(ErrorMessage = "Chủ đề không được để trống")]
        public string? Title { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Điện thoại")]
        [Required(ErrorMessage = "Điện thoại không được để trống")]
        public string? Phone { get; set; }

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string? Content { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Tên không được để trống")]
        public string? Name { get; set; }

        public ContactStatus Status { get; set; }

        /// <summary>
        /// Nội dung xử lý
        /// </summary>
        [Display(Name = "Nội dung xử lý")]
        public string? Comment { get; set; }

    }

    public enum ContactStatus
    {
        [Description("Khởi tạo")]
        Initial = 0,

        [Description("Đang xử lý")]
        InProgress = 1,

        [Description("Đã xử lý")]
        Resolved
    }
}
