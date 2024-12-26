using RauSach.Core.Services;

namespace RauSach.Web.Helpers
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailTemplate(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetTemplate(string filename)
        {
            var path = $"{_webHostEnvironment.WebRootPath}\\mail-templates\\{filename}";
            return File.ReadAllText(path);
        }
    }
}
