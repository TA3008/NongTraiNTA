namespace RauSach.Web.Models
{
    public class AccountMenuViewModel
    {
        public bool IsAuthenticated { get; set; }
        public bool HasAdminSitePermission { get; set; }
        public string Username { get; set; }
        public bool IsExternalLogin { get; set; }
    }
}
