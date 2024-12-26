using System.ComponentModel.DataAnnotations;

namespace RauSach.Web.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; }

        [Display(Name = "Điện thoại")]
        [Required(ErrorMessage = "Điện thoại không được để trống")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }
    }
}
