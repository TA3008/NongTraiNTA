using MongoDB.Bson.Serialization.Attributes;
using RauSach.Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace RauSach.Core.Models
{
    [BsonIgnoreExtraElements]
    public class Garden : BaseEntity
    {
        [Display(Name = "Tên vườn")]
        [Required(ErrorMessage = "Tên vườn không được để trống")]
        public string? Name { get; set; }

        /// <summary>
        /// Dien tich, m2
        /// </summary>
        [Display(Name = "Diện tích m2/tháng")]
        [Required(ErrorMessage = "Diện tích không được để trống")]
        public int Area { get; set; }

        /// <summary>
        /// So luong vuon
        /// </summary>
        [Display(Name = "Số lượng vườn")]
        public int Quantity { get; set; }

        /// <summary>
        /// Don gia (vnd)
        /// </summary>
        [Display(Name = "Giá thuê/tháng/m2 (đ)")]
        [Required(ErrorMessage = "Giá thuê không được để trống")]
        public long Price { get; set; }

        /// <summary>
        /// Giá để hiển thị trên web (discount)
        /// </summary>
        [Display(Name = "Giá fake (đ)")]
        public int FakePrice { get; set; }

        /// <summary>
        /// So luong loai rau
        /// </summary>
        [Display(Name = "Số lượng rau tối đa")]
        public int MaximumVegetable { get; set; }

        public string? ImageUrl { get; set; }

        /// <summary>
        /// Danh cho loai vuon VIP
        /// </summary>
        [Display(Name = "Cho phép chọn rau")]
        public bool CanCustomize { get; set; }
        public string? Thumbnail { get; set; }

        [Display(Name = "Trọng lượng rau mỗi lần vận chuyển (g)")]
        public int DeliveryWeight { get; set; }

        [Display(Name = "Sai số (+-g)")]
        public int AdjustWeight { get; set; }

        [Display(Name = "Số lần vận chuyển tối đa 1 tháng")]
        public int DeliveryTimePerMonth { get; set; }

        /// <summary>
        /// Thông tin vườn show lên trang
        /// </summary>
        [Display(Name = "Thông tin vườn")]
        public string? Details { get; set; }

        [Display(Name = "Danh sách mã vườn và diện tích (m2), ví dụ V01;100")]
        public string? GardenItems { get; set; }

        public List<GardenItem> GetGardenItems()
        {
            var items = new List<GardenItem>();
            if (string.IsNullOrWhiteSpace(GardenItems)) return items;

            foreach (var item in GardenItems.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Where(m => !string.IsNullOrWhiteSpace(m)))
            {
                var arr = item.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Where(m => !string.IsNullOrWhiteSpace(m)).Select(m => m.Trim()).ToList();

                if (arr.Count < 2) continue;

                var info = new GardenItem { Code = arr[0] };
                int.TryParse(arr[1], out var area);
                info.Area = area;
                items.Add(info);
            }
            return items;
        }

        public List<GardenItem> GetDistinctGardenItems()
        {
            var items = new List<GardenItem>();
            var gardenItems = GetGardenItems();
            if (gardenItems.Any())
            {
                foreach(var item in gardenItems)
                {
                    if(items.Any(x => x.Area == item.Area)) continue;
                    items.Add(item);
                }
            }
            return items.OrderBy(x => x.Area).ToList();
        }

        [BsonIgnore]
        public int MinArea
        {
            get
            {
                var areas = GetGardenItems().Select(x => x.Area);
                if (areas.Any())
                {
                    return areas.Min();
                }
                return 0;
            }
        }

        [BsonIgnore]
        public int MaxArea
        {
            get
            {
                var areas = GetGardenItems().Select(x => x.Area);
                if (areas.Any())
                {
                    return areas.Max();
                }
                return 0;
            }
        }
    }
}
