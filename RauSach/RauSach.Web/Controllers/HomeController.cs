using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;
using RauSach.Core.Models;
using System.Diagnostics;

namespace RauSach.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContactRepository _contactRepository;

        public HomeController(ILogger<HomeController> logger,
            IContactRepository contactRepository)
        {
            _logger = logger;
            _contactRepository = contactRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SaveContact(Contact model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
                _contactRepository.UpdateAsync(model).GetAwaiter().GetResult();
                TempData["success"] = true;
                return RedirectToAction("Contact");
            }
            return View("Contact", model);
        }
    }
}