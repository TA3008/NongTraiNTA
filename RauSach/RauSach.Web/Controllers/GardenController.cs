using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;

namespace RauSach.Web.Controllers
{
    public class GardenController : Controller
    {
        private readonly ILogger<GardenController> _logger;
        private readonly IGardenRepository _gardenRepository;
        private readonly IVegetableRepository _vegetableRepository;
        private readonly IVegetableComboRepository _vegetableComboRepository;

        public GardenController(ILogger<GardenController> logger,
            IGardenRepository gardenRepository,
            IVegetableRepository vegetableRepository,
            IVegetableComboRepository vegetableComboRepository)
        {
            _logger = logger;
            _gardenRepository = gardenRepository;
            _vegetableRepository = vegetableRepository;
            _vegetableComboRepository = vegetableComboRepository;
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult Detail(string url)
        {
            var garden = _gardenRepository.Find(x => x.FriendlyUrl == url && x.Deleted == false).FirstOrDefault();
            return View(garden);
        }
    }
}
