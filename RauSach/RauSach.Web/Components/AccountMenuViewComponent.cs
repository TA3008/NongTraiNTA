using Microsoft.AspNetCore.Mvc;
using RauSach.Core.Services;
using RauSach.Web.Models;

namespace RauSach.Web.Components
{
    public class AccountMenuViewComponent : ViewComponent
    {
        private readonly IUserGroupManager _userGroupManager;

        public AccountMenuViewComponent(IUserGroupManager userGroupManager)
        {
            _userGroupManager = userGroupManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var accountMenu = new AccountMenuViewModel()
            {
                IsAuthenticated = User.Identity?.IsAuthenticated == true,
                HasAdminSitePermission = User.Identity?.IsAuthenticated == true && _userGroupManager.GetAllRoles(User.Identity.Name).Any(),
                Username = User.Identity?.Name,
                IsExternalLogin = User.Identity?.IsAuthenticated == true && _userGroupManager.GetUser(User.Identity.Name)?.Logins?.Any() == true
            };
            return View(accountMenu);
        }
    }
}
