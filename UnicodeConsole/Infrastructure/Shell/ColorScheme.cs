using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32ConEcho;

namespace UnicodeConsole.Infrastructure.Shell
{
    public enum ColourScheme
    {
        WhiteOnBlack,
        BlackOnWhite
    }

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
        public static ANSIColour ToANSIColour(this MessageColour @this, ColourScheme scheme)
        {
            switch (scheme)
            {
                case ColourScheme.WhiteOnBlack:
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
                            throw new ArgumentException("No mapping for: " + @this + " in the scheme: " + scheme, "this");
                    }

                case ColourScheme.BlackOnWhite:
                    switch (@this)
                    {
                        case MessageColour.Background:
                            return ANSIColour.White;
                        case MessageColour.Text:
                            return ANSIColour.Black;
                        case MessageColour.Error:
                            return ANSIColour.Red;
                        case MessageColour.Warning:
                            return ANSIColour.Yellow;
                        case MessageColour.OK:
                            return ANSIColour.Green;

                        default:
                            throw new ArgumentException("No mapping for: " + @this + " in the scheme: " + scheme, "this");
                    }

                default:
                    throw new ArgumentException("Color scheme: " + scheme + " is not implemented!", "scheme");
            }
        }
    }
}
