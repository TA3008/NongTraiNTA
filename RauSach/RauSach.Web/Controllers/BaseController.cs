using Microsoft.AspNetCore.Mvc;

namespace RauSach.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string GetModalStateErrorMsg()
        {
            if (!ModelState.IsValid)
            {
                return string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            }
            return string.Empty;
        }
    }
}
