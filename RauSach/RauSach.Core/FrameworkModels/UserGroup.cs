using RauSach.Core.Models;

namespace RauSach.Core.FrameworkModels
{
    public class UserGroup : BaseEntity
    {
        public string? Name { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
