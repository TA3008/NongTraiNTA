using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace RauSach.Web.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public List<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
    }

    public class ManageUserViewModel
    {
        [Required(ErrorMessage = "Mật khẩu cũ bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới bắt buộc")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression("^[a-zA-Z0-9!@#$%^&*()_+\\-=\\[\\]{};':\"\\|,.<>\\/?]*$", ErrorMessage = "Mật khẩu phải có ít nhất 1 ký tự đặc biệt")]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu mới bắt buộc")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression("^[a-zA-Z0-9!@#$%^&*()_+\\-=\\[\\]{};':\"\\|,.<>\\/?]*$", ErrorMessage = "Mật khẩu phải có ít nhất 1 ký tự đặc biệt")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu mới chưa khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Vai trò")]
        public bool IsAdmin { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có độ dài từ {0} đến {2} kí tự.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu chưa khớp.")]
        public string ConfirmPassword { get; set; }
    }

    public class PhoneRegisterViewModel
    {
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhân mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu phải giống nhau")]
        public string PasswordConfirm { get; set; }
        
    }
}
