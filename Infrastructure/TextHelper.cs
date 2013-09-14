using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure
{
    static class TextHelper
    {
        /// <summary>
        /// Add Text with proper intentation and max width (using word breaker)
        /// TODO: proper indentation
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="text"></param>
        /// <param name="followLineSpace"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static StringBuilder AppendWithLineBreaking(this StringBuilder builder, string text, int followLineSpace, int space)
        {
            return builder.Append(text);
        }

        public static string PrefixedMultiLineString(this string longText, int bufferWidth, string prefixFormat, params object[] prefixArgs)
        {
            var builder = new StringBuilder(2 * bufferWidth).AppendFormat(prefixFormat, prefixArgs);
            var descSpace = (bufferWidth - builder.Length - 1).EnsurePositive().EnsureInt32();
            var approxNumberOfLines = (int)Math.Ceiling(1.0f * longText.Length / descSpace);
            builder.EnsureCapacity(approxNumberOfLines * bufferWidth);

            if (descSpace < 10)
                throw new Exception("No space for the long text: " + new { builder.Length, descSpace });

            return builder.AppendWithLineBreaking(longText, builder.Length, descSpace).ToString();
        }
    }
}
