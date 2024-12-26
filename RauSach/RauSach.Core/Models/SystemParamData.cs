namespace RauSach.Core.Models
{
    public class SystemParamData : BaseEntity
    {
        public string? Type { get; set; }
        public string? DataName { get; set; }
        public object? DataValue { get; set; }
        public string? Description { get; set; }
    }
}
