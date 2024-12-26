namespace RauSach.Web.Models
{
    public class ReportModel
    {
        public List<string> OrderStatusLabels { get; set; } = new List<string>();
        public List<int> OrderStatusCounts { get; set; } = new List<int>();
    }
}
