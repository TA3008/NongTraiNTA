using Rausach.Common.Extensions;
using RauSach.Core.ValueObjects;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RauSach.Core.Models
{
    public class Order : BaseEntity
    {
        public int CurrentCount { get; set; }
        public string? Code { get; set; }
        public string? CustomerNote { get; set; }
        public string? AdminNote { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        public long Price { get; set; }

        /// <summary>
        /// Username của customer mua hàng
        /// </summary>
        public string? Username { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? CustomerAddress { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string? CustomerPhone { get; set; }
        public string? GardenCode { get; set; }
        public Guid? FarmerId { get; set; }
        public Guid? SaleId { get; set; }
        public string? FarmerName { get; set; }
        [Required(ErrorMessage = "Ảnh chụp màn hình thanh toán không được để trống")]
        public string ImageUrl { get; set; }

        public DateTime IssuedDate { get; set; }
        public DateTime ExpiredDate { get; set; }

        public List<OrderStatusDetails> StatusHistories { get; set; } = new List<OrderStatusDetails>();

        public OrderStatus Status { get; set; }
        public PaymentType PaymentType { get; set; }

        public Garden Garden { get; set; } = new Garden();

        public VegetableCombo Combo { get; set; } = new VegetableCombo();

        public Voucher Voucher { get; set; } = new Voucher();

        public static IEnumerable<(OrderStatus value, string text)> GetPossibleStatuses()
        {
            var values = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();
            foreach (var item in values)
            {
                yield return new (item, item.GetEnumDescription());
            }
        }
    }
}
