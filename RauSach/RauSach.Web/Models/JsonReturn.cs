namespace RauSach.Web.Models
{
    public class JsonReturn
    {
        public bool success { get; set; }
        public string message { get; set; }

        public JsonReturn(bool _success)
        {
            success = _success;
        }

        public JsonReturn(bool _success, string _message)
        {
            success = _success;
            message = _message;
        }
    }
}
