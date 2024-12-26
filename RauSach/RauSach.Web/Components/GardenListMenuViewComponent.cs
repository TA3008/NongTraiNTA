using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;

namespace RauSach.Web.Components
{
    public class GardenListMenuViewComponent : ViewComponent
    {
        private readonly IGardenRepository _gardenRepository;

        public GardenListMenuViewComponent(IGardenRepository gardenRepository)
        {
            _gardenRepository = gardenRepository;
        }

        public IViewComponentResult Invoke()
        {
            var gardens = _gardenRepository.Find(x => x.Deleted == false).ToList();

            return View(gardens);
        }
    }
}
