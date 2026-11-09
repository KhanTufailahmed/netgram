using System.Net;
using System.Text.RegularExpressions;
using Ganss.Xss;

namespace netgram.Services
{
    public static class PostContentSanitizer
    {
        public static string Sanitize(string content)
        {
            return new HtmlSanitizer().Sanitize(content);
        }

        public static string ToPlainText(string content)
        {
            var safeHtml = Sanitize(content);
            var plainText = Regex.Replace(safeHtml, "<[^>]+>", " ");
            return Regex.Replace(WebUtility.HtmlDecode(plainText), @"\s+", " ").Trim();
        }
    }
}
