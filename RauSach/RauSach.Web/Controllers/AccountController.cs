using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Repositories;
using RauSach.Core.Services;
using RauSach.Web.Areas.Admin.Models;
using RauSach.Web.Models;
using System.Security.Claims;

namespace RauSach.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserGroupManager _userGroupManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;

        [TempData]
        public string ErrorMessage { get; set; }

        public AccountController(ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUserGroupManager userGroupManager,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _userGroupManager = userGroupManager;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var hasAdminPermission = _userGroupManager.HasPermission(User.Identity.Name, new string[]
                {
                    RoleList.Admin, RoleList.Account, RoleList.Product, RoleList.Content
                });
                return hasAdminPermission ? RedirectToAction("Index", "Home", new { Area = "Admin" }) : RedirectToAction("Index", "Home");
            }
            await CreateAdminUserIfNeeded();
            var model = new LoginViewModel();
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> PhoneRegister()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var hasAdminPermission = _userGroupManager.HasPermission(User.Identity.Name, new string[]
                {
                    RoleList.Admin, RoleList.Account, RoleList.Product, RoleList.Content
                });
                return hasAdminPermission ? RedirectToAction("Index", "Home", new { Area = "Admin" }) : RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PhoneRegister(PhoneRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Phone);
            if(user != null)
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã được đăng ký trước đó.");
                return View(model);
            }
            var admin = new User
            {
                UserName = model.Phone,
                IsLocked = false,
                PhoneNumber = model.Phone
            };
            await _userManager.CreateAsync(admin, model.Password);

            var result = await _signInManager.PasswordSignInAsync(model.Phone, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Redirect("/");
            }

            TempData["success"] = true;
            return View(model);
        }

        // This method handles the POST request when the user submits the login form.
        // It checks if the form data is valid, and if so, it attempts to sign in the user.
        // If the user is locked, an error message is added to the model state and the view is returned.
        // If the sign-in attempt is successful, it checks if the user has the required permissions.
        // If the user has the required permissions, it redirects to the login action.
        // Otherwise, it redirects to the return URL or the home page.
        // If the sign-in attempt fails, an error message is added to the model state and the view is returned.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            // Check if the form data is valid
            if (!ModelState.IsValid)
            {
                // If the form data is not valid, return the view with the model
                return View(model);
            }

            // Find the user by their username
            var user = await _userManager.FindByNameAsync(model.UserName);

            // Check if the user is locked
            if (user?.IsLocked == true)
            {
                // If the user is locked, add an error message to the model state and return the view with the model
                ModelState.AddModelError("", $"Tài khoản {model.UserName} đã bị khóa.");
                return View(model);
            }

            // Attempt to sign in the user with the provided username and password
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);

            // Check if the sign-in attempt was successful
            if (result.Succeeded)
            {
                // Check if the user has the required permissions
                var hasPermission = _userGroupManager.HasPermission(model.UserName, new string[]
                {
                    RoleList.Admin, RoleList.Account, RoleList.Product, RoleList.Content
                });

                // If the user has the required permissions, redirect to the login action
                if (hasPermission)
                {
                    return RedirectToAction("Login");
                }

                // If the user does not have the required permissions, redirect to the return URL or the home page
                returnUrl = returnUrl ?? "/";
                return Redirect(returnUrl);
            }

            // If the sign-in attempt fails, add an error message to the model state and return the view with the model
            ModelState.AddModelError("", "Sai mật khẩu hoặc tên đăng nhập.");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginHandler", controller: "Account", new { returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginHandler(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            // If the user does not have an account, then ask the user to create an account.
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = new User { UserName = email, Email = $"{email}".ToLower(), Updated = DateTimeExtensions.UTCNowVN };
                var creatingResult = await _userManager.CreateAsync(user);
                if (creatingResult.Succeeded)
                {
                    creatingResult = await _userManager.AddLoginAsync(user, info);
                    if (creatingResult.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
            }

            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        public ActionResult Unauthorize(string returnUrl) => View(model: returnUrl);

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ManageUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null) return null;
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return Redirect("/");
                }
                AddErrors(result);
            }
            return View(model);
        }

        public ActionResult Profile()
        {
            var user = _userRepository.GetByUsername(User.Identity.Name);
            var model = new UserViewModel()
            {
                UserName= user.UserName,
                Email= user.Email,
                PhoneNumber= user.PhoneNumber,
                Address = user.Address ?? "",
                Name = user.Name
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Profile(UserViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = _userRepository.GetByUsername(User.Identity.Name);
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.Name = model.Name;
                _userRepository.UpdateAsync(user);
                TempData["success"] = true;
                return RedirectToAction("Profile");
            }
            return View(model);
        }


        private async Task CreateAdminUserIfNeeded()
        {
            var username = "admin";
            var admin = await _userManager.FindByNameAsync(username);
            if (admin == null)
            {
                admin = new User
                {
                    UserName = username,
                    Email = $"{username}@email.com",
                    IsLocked = false,
                    CustomRoles = new List<string> { RoleList.Admin }
                };
                await _userManager.CreateAsync(admin, "1");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
