using RauSach.Core.Models;

namespace RauSach.Web.Models
{
    public class PlantingModel
    {
        public Order Order { get; set; } = new Order();
        public bool StartPlanting { get; set; }
        public DateTime StartDate { get; set; }
        public Vegetable Vegetable { get; set; } = new Vegetable();
    }
}
