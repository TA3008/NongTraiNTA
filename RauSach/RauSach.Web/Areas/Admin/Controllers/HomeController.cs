using Microsoft.AspNetCore.Mvc;
using Rausach.Common.Extensions;
using RauSach.Application.Repositories;
using RauSach.Web.Areas.Admin.Models;
using RauSach.Web.Models;
using System.Diagnostics;

namespace RauSach.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrderRepository _orderRepository;

        public HomeController(ILogger<HomeController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            var orders = _orderRepository.GetAll().GroupBy(info => info.Status)
                        .Select(group => new
                        {
                            Status = group.Key,
                            Count = group.Count()
                        });
            var model = new ReportModel();
            foreach (var item in orders)
            {
                var label = $"{item.Status.GetEnumDescription()} ({item.Count})";
                model.OrderStatusLabels.Add(label);
                model.OrderStatusCounts.Add(item.Count);
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
