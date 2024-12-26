using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RauSach.Core.Services;

namespace RauSach.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class WebAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _filterParams;

        public WebAuthorizeAttribute(params string[] filterParams)
        {
            _filterParams = filterParams;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Account", action = "Login" }));
                return;
            }
            
            var userService = context.HttpContext.RequestServices.GetService<IUserGroupManager>();

            var isAuthorized = userService.HasPermission(user.Identity.Name, _filterParams);
            if (!isAuthorized)
            {
                context.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { area = "", controller = "Account", action = "Unauthorize", returnUrl = context.HttpContext.Request.GetEncodedUrl() }));
                //context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                return;
            }
        }

        public string[] GetParams() => _filterParams;
    }
}
