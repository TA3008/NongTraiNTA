namespace Rausach.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime UTCNowVN
        {
            get { return DateTime.UtcNow.AddHours(7); }
        }
    }
}