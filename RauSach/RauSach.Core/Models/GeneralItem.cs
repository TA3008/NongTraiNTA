namespace RauSach.Core.Models
{
    public class GeneralItem : BaseEntity
    {
        public string? Type { get; set; }
        public string Title { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
        public string? Thumbnail { get; set; }
    }
}
