using MongoDB.Bson.Serialization.Attributes;
using Rausach.Common.Extensions;
using RauSach.Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace RauSach.Core.Models
{
    public class Vegetable : BaseEntity
    {
        [Display(Name = "Tên rau")]
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// So ngay thu hoach
        /// </summary>
        [Display(Name = "Thời gian thu hoạch lần đầu")]
        public int LifeDay { get; set; }

        /// <summary>
        /// Có thể thu hoạch nhiều lần hay không
        /// </summary>
        [Display(Name = "Có thể thu hoạch nhiều lần")]
        public bool CanHarvestManyTimes { get; set; }

        [Display(Name = "Thời gian thu hoạch lần sau")]
        public int LifeDayRecycle { get; set; }

        public string? ImageUrl { get; set; }
        public string? Thumbnail { get; set; }

        /// <summary>
        /// Dien tich trồng
        /// </summary>
        [Display(Name = "Diện tích trồng")]
        public int Area { get; set; }

        [Display(Name = "Năng suất g/m2")]
        public float GramPerM2 { get; set; }

        public VegetableDelivery Delivery { get; set; } = new VegetableDelivery();

        [BsonIgnore]
        public int HarvestProceedLife
        {
            get
            {
                if (Delivery.StartDate != DateTime.MinValue && DateTimeExtensions.UTCNowVN > Delivery.StartDate)
                {
                    var days = Math.Abs((DateTimeExtensions.UTCNowVN - Delivery.StartDate).Days);
                    if (days <= LifeDay)
                    {
                        return (int)(((double)days / LifeDay) * 100);
                    }
                    else if(days > LifeDay && (CanHarvestManyTimes || (!CanHarvestManyTimes && Delivery.Status.DeliveryState == DeliveryState.Pendding)))
                    {
                        return 100;
                    }
                }
                return 0;
            }
        }

        [BsonIgnore]
        public string HarvestProceedLifeStr
        {
            get
            {
                if (Delivery.StartDate != DateTime.MinValue && DateTimeExtensions.UTCNowVN > Delivery.StartDate)
                {
                    var days = Math.Abs((DateTimeExtensions.UTCNowVN - Delivery.StartDate).Days);
                    if (days <= LifeDay)
                    {
                        return $"{days}/{LifeDay}";
                    }
                    else if (days > LifeDay && (CanHarvestManyTimes || (!CanHarvestManyTimes && Delivery.Status.DeliveryState == DeliveryState.Pendding)))
                    {
                        return $"{LifeDay}/{LifeDay}"; 
                    }
                }
                return $"0/{LifeDay}";
            }
        }
    }
}
