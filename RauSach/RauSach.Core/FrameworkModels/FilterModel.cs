namespace RauSach.Core.FrameworkModels
{
    public class FilterModel
    {
        public string? query { get; set; }
        public string? custom { get; set; }
        public string? status { get; set; }
        public int start { get; set; }
        public int limit { get; set; } = 200;
        public int asc { get; set; } = 1;
        public string? orderby { get; set; }
        public bool isAdmin { get; set; }
    }
}
