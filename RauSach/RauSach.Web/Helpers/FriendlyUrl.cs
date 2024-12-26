namespace RauSach.Web.Helpers
{
    /// <summary>
    /// Làm các url để seo tốt hơn
    /// </summary>
    public static class FriendlyUrl
    {
        public static string ArticleDetailFrm = "/tin/{url}";
        public static string ArticleListFrm = "/dach-sach-tin";
        public static string ShopFrm = "/dat-hang/vuon";
        public static string MyOrderFrm = "/don-hang-cua-toi";
        public static string ProfileFrm = "/thong-tin-cua-toi";
        public static string MyGardensFrm = "/vuon-cua-toi";
        public static string MyGardenDetailFrm = "/vuon-cua-toi/{code}";
        public static string GardenDetailFrm = "/vuon/{url}";
        public static string GardenListFrm = "/danh-sach-vuon";
        public static string AboutUsFrm = "/ve-chung-toi";
        public static string ContactFrm = "/lien-he";

        //tạm thời để thế này mai demo, sửa sau
        public static string Login { 
            get { return "/account/login"; } 
        }

        public static string Home
        {
            get { return "/home/index"; }
        }

        public static string ArticleList()
        {
            return ArticleListFrm; 
        }

        public static string ArticleDetail(string url)
        {
            return TrimStar(ArticleDetailFrm).Replace("{url}", url); 
        }

        public static string Shop()
        {
            return ShopFrm;
        }

        public static string MyOrder()
        {
            return MyOrderFrm;
        }

        /// <summary>
        /// Các vườn của tôi
        /// </summary>
        /// <returns></returns>
        public static string MyGardens()
        {
            return MyGardensFrm;
        }

        /// <summary>
        /// Chi tiết vườn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string MyGardenDetail(string code)
        {
            return MyGardenDetailFrm.Replace("{code}", code);
        }

        public static string GardenDetail(string url)
        {
            return GardenDetailFrm.Replace("{url}", url);
        }

        public static string GardenList()
        {
            return GardenListFrm;
        }

        public static string AboutUs()
        {
            return AboutUsFrm;
        }

        public static string Contact()
        {
            return ContactFrm;
        }

        public static string Profile()
        {
            return ProfileFrm;
        }

        #region admin

        public static string Admin
        {
            get { return "/admin/home/index"; }
        }

        #endregion

        private static string TrimStar(string url)
        {
            return url.Replace("*", "");
        }
    }
}
