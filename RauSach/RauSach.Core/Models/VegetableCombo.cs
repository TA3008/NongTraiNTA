using MongoDB.Bson.Serialization.Attributes;
using SharpCompress.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RauSach.Core.Models
{
    /// <summary>
    /// Goi y combo
    /// </summary>
    public class VegetableCombo : BaseEntity
    {
        [Display(Name = "Tên combo")]
        [Required]
        public string? Name { get; set; }
        public List<Vegetable> Vegetables { get; set; } = new List<Vegetable> { };

        [Display(Name = "Combo gợi ý")]
        public bool IsSuggestion { get; set; }

        [Display(Name = "Thuộc loại vườn")]
        public Guid GardenId { get; set; }

        [BsonIgnore]
        public bool? CanCustomize { get; set; }
    }
}
