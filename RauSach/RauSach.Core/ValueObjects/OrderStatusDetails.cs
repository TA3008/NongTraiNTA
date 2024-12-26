namespace RauSach.Core.ValueObjects
{
    public class OrderStatusDetails
    {
        public DateTime ActionTime { get; set; }
        public OrderStatus Status { get; set; }

        public string Author { get; set; }
    }
}
