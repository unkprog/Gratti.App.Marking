using System;
using System.Text;

namespace Gratti.App.Marking.Extensions
{
    public static partial class Object
    {
        public static string EscapeChars(this string inString)
        {
            return inString.Replace("!", "%21").Replace(@"\\", "%5C").Replace(@"""", "%22").Replace("%", "%25").Replace("&", "%26").Replace("'", "%27")
                .Replace("*", "%2A").Replace("+", "%2B").Replace("-", "%2D").Replace(".", "%2E").Replace("/", "%2F").Replace("_", "%5F").Replace(",", "%2C")
                .Replace(":", "%3A").Replace(";", "%3B").Replace("=", "%3D").Replace("<", "%3C").Replace(">", "%3E").Replace("?", "%3F")
                .Replace("(", "%28").Replace(")", "%29");
        }

        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        /// <summary>
        /// Escapes a string according to the URI data string rules given in RFC 3986.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        /// host actually having this configuration element present.
        /// </remarks>
        public static string EscapeUriDataStringRfc3986(this string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }

        public static string ReplaceLineEndings(this string Self, string replacementText)
        {
           return Self.Replace("\r\n", replacementText)
                      .Replace("\r", replacementText)
                      .Replace("\n", replacementText);
        }
    }
}
