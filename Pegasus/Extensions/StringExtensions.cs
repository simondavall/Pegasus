using System.Text.RegularExpressions;

namespace Pegasus.Extensions
{
    public static class StringExtensions
    {
        public static string Linkify(this string value)
        {
            // unescaped (?:[^!\\(\"'\\)])((https?|ftp|file)\:\/\/([0-9a-zA-Z](?:[-.\w]*[0-9a-zA-Z])*(?:(0-9)*)*(:\d+)?(?:\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_=]*)?))
            const string regexPattern = "(?:[^!\\(\"'\\)])((https?|ftp|file)\\:\\/\\/([0-9a-zA-Z](?:[-.\\w]*[0-9a-zA-Z])*(?:(0-9)*)*(:\\d+)?(?:\\/?)([a-zA-Z0-9\\-\\.\\?\\,\'\\/\\\\+&amp;%\\$#_=]*)?))";
            const string replacementPattern = " <a href=\"$1\">$3</a>";

            var newValue = Regex.Replace(value, regexPattern, replacementPattern);

            return newValue;
        }
    }
}
