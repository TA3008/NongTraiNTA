namespace RauSach.Web.Models
{
    public class GeneralItemGardenViewModel
    {
        public Guid FarmerId { get; set; }
        public string GardenName { get; set; }
        public List<string> GardenCodes { get; set; }
    }
}
