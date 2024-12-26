using System.ComponentModel.DataAnnotations;

namespace RauSach.Core.ValueObjects
{
    public class GardenItem
    {
        [Display(Name = "Mã vườn")]
        public string Code { get; set; }

        [Display(Name = "Diện tích (m2)")]
        public int Area { get; set; }
    }
}
