using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Repositories;
using RauSach.Core.Services;
using RauSach.Web.Areas.Admin.Models;
using RauSach.Web.Filters;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [WebAuthorize(RoleList.Admin, RoleList.Customer)]
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupManager _userGroupManager;
        private readonly IUserGroupRepository _groupRepository;

        public AccountController(ILogger<AccountController> logger,
            UserManager<User> userManager,
            IUserRepository userRepository,
            IUserGroupManager userGroupManager,
            IUserGroupRepository groupRepository
            )
        {
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
            _userGroupManager = userGroupManager;
            _groupRepository = groupRepository;
        }

        public async Task<IActionResult> Employees(FilterModel model)
        {
            if (model == null) model = new FilterModel();
            if (!_userGroupManager.HasPermission(User.Identity.Name, RoleList.Admin))
                model.custom = "Khách hàng";
            ViewBag.SearchModel = model;
            var users = await _userRepository.FindAsync(model);
            return View(users);
        }

        public ActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEmployee(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email ?? "email@email.com",
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Employees));
                    }

                    AddErrors(result);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra, liên hệ nhà phát triển phần mềm để được hỗ trợ.");
            }
            return View(model);
        }

        [WebAuthorize(RoleList.Admin)]
        public async Task<IActionResult> LockUser(string id, bool isLocked)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                user.IsLocked = isLocked;
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Employees));
            }

            return RedirectToAction(nameof(Employees));
        }

        [WebAuthorize(RoleList.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string returnUrl)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, "nongtraintc");
                TempData["Success"] = $"{DateTimeExtensions.UTCNowVN.ToString("dd/MM/yyyy hh:mm:ss")}: Reset mật khẩu thành công";
            }
            catch (Exception e)
            {
                TempData["Error"] = $"{DateTimeExtensions.UTCNowVN.ToString("dd/MM/yyyy hh:mm:ss")}: Lỗi xảy ra khi reset mật khẩu: {e.Message}";
                return Redirect(returnUrl);
            }

            return Redirect(returnUrl);
        }

        [WebAuthorize(RoleList.Admin)]
        [HttpGet]
        public async Task<ActionResult> AssignRoles(string id, bool? isGroup)
        {
            var controllers = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type));
            var allRoles = new List<string>();
            foreach (var conller in controllers)
            {
                var att = Attribute.GetCustomAttributes(conller);
                foreach (var item in att)
                {
                    var roles = (item as Filters.WebAuthorizeAttribute)?.GetParams();
                    if (roles?.Any() == true) allRoles.AddRange(roles);
                }
            }
            var assignedRoles = new List<string>();
            if (isGroup.HasValue)
            {
                var groupdId = new Guid(id);
                var group = await _groupRepository.GetAsync(groupdId);
                assignedRoles = group.Roles;
            }
            else
            {
                var user = await _userRepository.GetByIdAsync(id);
                assignedRoles = user.CustomRoles;
            }

            (string ObjectId, List<KeyValuePair<string, bool>> Roles) model = (id, new List<KeyValuePair<string, bool>>());
            foreach (var item in allRoles)
            {
                model.Roles.Add(new KeyValuePair<string, bool>(item, assignedRoles.Contains(item)));
            }
            model.Roles = model.Roles.Distinct().ToList();
            return View(model);
        }

        [WebAuthorize(RoleList.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignRoles(string Item1, List<string> roles, bool? isGroup, string returnUrl)
        {
            if (isGroup.HasValue)
            {
                var groupdId = new Guid(Item1);
                await _userGroupManager.SetGroupRolesAsync(groupdId, roles);
            }
            else
            {
                await _userGroupManager.SetUserRolesAsync(Item1, roles);
            }
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction(nameof(Employees));
            else return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(string id)
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];
            var user = await _userRepository.GetByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDetails(User model, string returnUrl)
        {
            if (model.Email != null)
                model.Email = model.Email.Trim().ToLower();

            var user = await _userRepository.GetByIdAsync(model.Id.ToString());
            user.Email = model.Email ?? user.Email;
            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Updated = DateTimeExtensions.UTCNowVN;

            var emailExisted = _userRepository.Find(m => m.Email != null && m.Email == model.Email && m.Id != model.Id).Any();
            if (emailExisted)
            {
                ViewBag.Error = $"Email {model.Email} đã tồn tại";
                return View(user);
            }

            await _userRepository.UpdateAsync(user);
            return Redirect(returnUrl);
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
