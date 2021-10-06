using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Pegasus.Extensions
{
    public static class StringExtensions
    {
        public static string Linkify(this string value)
        {
            // unescaped (?:[^!\\(\"'\\)])((https?|ftp|file)\:\/\/([0-9a-zA-Z](?:[-.\w]*[0-9a-zA-Z])*(?:(0-9)*)*(:\d+)?(?:\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_=]*)?))
            const string regexPattern = "(?:[^!\\(\"'\\)])((https?|ftp|file)\\:\\/\\/([0-9a-zA-Z](?:[-.\\w]*[0-9a-zA-Z])*(?:(0-9)*)*(:\\d+)?(?:\\/?)([a-zA-Z0-9\\-\\.\\?\\,\'\\/\\\\+&amp;%\\$#_=]*)?))";
            const string replacementPattern = " <a href=\"$1\">$3</a>";

            if (string.IsNullOrEmpty(value))
                return value;

            var newValue = Regex.Replace( " " + value, regexPattern, replacementPattern);

            return newValue.TrimStart();
        }

        /// <summary>
        /// Prepares a string content to be displayed as raw html. This included HtmlEncoding, changing NewLine chars to <br/>
        /// and detecting https:// web links and enclosing them in anchor tags.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string PreparedHtml(this string value)
        {
            return HttpUtility.HtmlEncode(value).Linkify()?.Replace(Environment.NewLine, "<br/>");
        }
    }
}
