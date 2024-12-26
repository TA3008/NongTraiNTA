using Microsoft.AspNetCore.Mvc;
using RauSach.Application.Repositories;
using RauSach.Core.Models;

namespace RauSach.Web.Components
{
    public class OrderComboListViewComponent : ViewComponent
    {
        private readonly IGardenRepository _gardenRepository;
        private readonly IVegetableComboRepository _comboRepository;
        private readonly IVegetableRepository _vegetableRepository;

        public OrderComboListViewComponent(IGardenRepository gardenRepository,
            IVegetableComboRepository comboRepository,
            IVegetableRepository vegetableRepository)
        {
            _gardenRepository = gardenRepository;
            _comboRepository = comboRepository;
            _vegetableRepository = vegetableRepository;
        }

        public IViewComponentResult Invoke(Guid gardenId)
        {
            var combos = _comboRepository.Find(x => x.GardenId == gardenId && x.Deleted != true).ToList();
            var garden = _gardenRepository.Get(gardenId);
            if(garden == null || combos.Any() == false)
            {
                return View();
            }

            if (garden.CanCustomize)
            {
                var vegetables = _vegetableRepository.GetAll();
                combos.Add(new VegetableCombo()
                {
                    Vegetables = vegetables,
                    Name = "Tuỳ chọn",
                    GardenId = gardenId,
                    CanCustomize = true
                });
            }

            return View(combos);
        }

        private int TotalArea(List<Vegetable> vegetables)
        {
            var total = 0;
            vegetables.ForEach(x => total += x.Area);
            return total;
        }
    }
}
