using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;

namespace RauSach.Web.Components
{
    public class GardenListViewComponent : ViewComponent
    {
        private readonly IGardenRepository _gardenRepository;

        public GardenListViewComponent(IGardenRepository gardenRepository)
        {
            _gardenRepository = gardenRepository;
        }

        public IViewComponentResult Invoke(Guid? ignoreGardenId)
        {
            var gardens = _gardenRepository.Find(x => x.Deleted == false).ToList();

            if(ignoreGardenId != null)
            {
                gardens = gardens.Where(x => x.Id != ignoreGardenId).ToList();
            }

            return View(gardens);
        }
    }
}
