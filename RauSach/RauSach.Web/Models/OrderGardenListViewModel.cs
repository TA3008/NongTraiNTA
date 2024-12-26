using RauSach.Core.Models;

namespace RauSach.Web.Models
{
    public class OrderGardenListViewModel
    {
        public List<Garden> Gardens { get; set; }

        /// <summary>
        /// Id của vườn đã chọn
        /// </summary>
        public Guid SelectedId { get; set; }
    }
}
