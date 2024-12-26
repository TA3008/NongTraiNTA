namespace RauSach.Web.Models
{
    public class HarvestedVegetableViewModel
    {
        /// <summary>
        /// Vegetable id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Khối lượng đã thu hoạch g
        /// </summary>
        public int HarvestedGr { get; set; }
    }
}
