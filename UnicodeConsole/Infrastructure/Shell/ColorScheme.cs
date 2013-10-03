using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32ConEcho;

namespace UnicodeConsole.Infrastructure.Shell
{
    public enum MessageColour
    {
        Background,
        AutoText,
        Error,
        OK,
        Warning,
        Text
    }

    public static class MessageColorEx
    {
        public static ANSIColour ToANSIColour(this MessageColour @this)
        {
            switch (@this)
            {
                case MessageColour.Background:
                    return ANSIColour.Black;
                case MessageColour.Text:
                    return ANSIColour.White;
                case MessageColour.Error:
                    return ANSIColour.Red;
                case MessageColour.Warning:
                    return ANSIColour.Yellow;
                case MessageColour.OK:
                    return ANSIColour.Green;

                default:
                    throw new ArgumentException("No mapping for: " + @this, "this");
            }
        }
    }
}
