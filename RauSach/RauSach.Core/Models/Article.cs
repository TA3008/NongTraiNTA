using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RauSach.Core.Models
{
    public class Article : BaseEntity
    {
        [Display(Name = "Tên bài viết")]
        [Required]
        public string? Name { get; set; }

        [Display(Name = "Hình ảnh")]
        [Required]
        public string? Img { get; set; }

        [Display(Name = "Nội dung")]
        [Required]
        public string? Content { get; set; }
        [Display(Name = "Xuất bản")]
        public bool IsPublished { get; set; }
        public string? Thumbnail { get; set; }

    }
}
