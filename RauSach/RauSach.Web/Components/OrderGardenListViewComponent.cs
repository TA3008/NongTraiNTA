using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;
using RauSach.Web.Models;

namespace RauSach.Web.Components
{
    public class OrderGardenListViewComponent : ViewComponent
    {
        private readonly IGardenRepository _gardenRepository;

        public OrderGardenListViewComponent(IGardenRepository gardenRepository)
        {
            _gardenRepository = gardenRepository;
        }

        public IViewComponentResult Invoke(Guid selectedGardenId)
        {
            var gardens = _gardenRepository.GetAll().Where(m => !m.Deleted).ToList();

            var model = new OrderGardenListViewModel()
            {
                Gardens = gardens,
                SelectedId = selectedGardenId
            };

            return View(model);
        }
    }
}
