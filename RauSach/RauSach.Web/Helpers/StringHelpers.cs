using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace RauSach.Web.Helpers
{
    public static class StringHelpers
    {
        private static readonly Random random = new Random();

        private static readonly string[] VietnameseSigns = new string[]
                                    {
                                        "aAeEoOuUiIdDyY",

                                        "áàạảãâấầậẩẫăắằặẳẵ",

                                        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

                                        "éèẹẻẽêếềệểễ",

                                        "ÉÈẸẺẼÊẾỀỆỂỄ",

                                        "óòọỏõôốồộổỗơớờợởỡ",

                                        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

                                        "úùụủũưứừựửữ",

                                        "ÚÙỤỦŨƯỨỪỰỬỮ",

                                        "íìịỉĩ",

                                        "ÍÌỊỈĨ",

                                        "đ",

                                        "Đ",

                                        "ýỳỵỷỹ",

                                        "ÝỲỴỶỸ"
                                    };

        private static readonly string SpecialChars = "~!@#$%^&*()`[]\\|;'\":<>/?.,“”";

        /// <summary>
        /// Loại bỏ dấu
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSign4VietnameseString(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            return str;

        }

        public static string ReplaceSpecialChar(string str, char charReplace)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
            for (int i = 0; i < SpecialChars.Length; i++)
            {
                str = str.Replace(SpecialChars[i], charReplace);
            }

            return str;
        }

        /// <summary>
        /// Đường link thân thiện
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToFriendlyUrl(string str)
        {
            string url = RemoveSign4VietnameseString(str).Replace(' ', '-');
            url = ReplaceSpecialChar(url, '-');
            while (url.Contains("--"))
            {
                url = url.Replace("--", "-");
            }
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Có đúng định dạng trang aspx hay ko
        /// abc.aspx
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public static bool IsValidPageName(string pageName)
        {
            string pattern = @"^[a-zA-Z0-9_]+\.html$";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            return rgx.IsMatch(pageName);
        }

        /// <summary>
        /// Bo cac tag html, tra ve thuan text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <Modified>
        /// Name     Date         Comments
        /// trungtq  2/9/2012   created
        /// </Modified>
        public static string StripHtml(string text)
        {
            string ret = string.Empty;
            try
            {
                string s = RemoveMultipleWhitespace(text.Trim());
                string strip = Regex.Replace(s, @"<(.|\n)*?>", string.Empty);

                if (!string.IsNullOrEmpty(strip))
                {
                    ret = strip;
                }
                else
                {
                    ret = EncodeHTMLTag(text);
                }
            }
            catch { }

            return ret;
        }

        /// <summary>
        /// Removes the script tag.
        /// </summary>
        /// <param name="strData">Doan text can remove</param>
        /// <returns></returns>
        /// <Modified>
        /// Name     Date         Comments
        /// trungtq  2/9/2012   created
        /// </Modified>
        public static string RemoveScriptTag(string strData)
        {
            strData = Regex.Replace(strData, "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return strData;
        }

        /// <summary>
        /// Kiem tra xem doan text co chua cac the html ko?
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///   <c>true</c> doan ma co chua the html ; otherwise <c>false</c>.
        /// </returns>
        /// <Modified>
        /// Name     Date         Comments
        /// trungtq  2/9/2012   created
        /// </Modified>
        public static bool IsContainsHTMLTag(string text)
        {
            Regex regex = new Regex(@"<(.|\n)*?>", RegexOptions.IgnoreCase);

            return regex.IsMatch(text);
        }

        /// <summary>
        /// Encodes the HTML tag.
        /// </summary>
        /// <param name="text">Doan text can encode.</param>
        /// <returns></returns>
        /// <Modified>
        /// Name     Date         Comments
        /// trungtq  2/9/2012   created
        /// </Modified>
        public static string EncodeHTMLTag(string text)
        {
            if (text.IndexOf('<') >= 0)
            {
                return HttpUtility.HtmlEncode(text);
            }
            return text;
        }

        /// <summary>
        /// Replaces the single quotes.
        /// fix quotes for SQL insertion...
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <Modified>
        /// Name     Date         Comments
        /// trungtq  15/8/2012   created
        /// </Modified>
        public static string ReplaceSingleQuotes(string text)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                return result;
            }
            result = text.Replace("'", "''").Trim();

            return result;
        }

        /// <summary>
        /// Removes multiple single quote ' characters from a string.
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <returns>
        /// The remove multiple single quotes.
        /// </returns>
        public static string RemoveMultipleSingleQuotes(string text)
        {
            string result = String.Empty;
            if (String.IsNullOrEmpty(text))
            {
                return result;
            }

            var r = new Regex(@"\'");
            return r.Replace(text, @"'");
        }

        /// <summary>
        /// Removes multiple whitespace characters from a string.
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <returns>
        /// The remove multiple whitespace.
        /// </returns>
        public static string RemoveMultipleWhitespace(string text)
        {
            string result = String.Empty;
            if (String.IsNullOrEmpty(text))
            {
                return result;
            }

            var r = new Regex(@"\s+");
            return r.Replace(text, @" ");
        }

        /// <summary>
        /// Bo tat ca khoang trang khoang trang trong chuoi truyen vao
        /// 29R2 7314 -> 29R27314
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <returns>
        /// The remove multiple whitespace.
        /// </returns>
        public static string RemoveWhitespace(string text)
        {
            string result = String.Empty;
            if (String.IsNullOrEmpty(text))
            {
                return result;
            }

            var r = new Regex(@"\s+");
            return r.Replace(text, string.Empty);
        }

        /// <summary>
        /// Xóa bỏ khoảng trắng, xuống dòng
        /// </summary>
        /// <param name="literal"></param>
        /// <returns></returns>
        public static string CleanWhiteSpace(string text)
        {
            string result = text;
            result = Regex.Replace(text, @"\s*\n\s*", "\n", RegexOptions.Multiline);
            result = Regex.Replace(text, @"\s{2,}", " ", RegexOptions.Multiline);

            return result;
        }

        public static string RandomChars(int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append(RandomChar());
            }
            return builder.ToString();
        }

        private static char RandomChar()
        {
            string str = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM123456789!@#$%^&*()_+";
            int i = random.Next(0, str.Length);
            return str[i];
        }
    }
}
