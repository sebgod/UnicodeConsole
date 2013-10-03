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
        TerminalBlack
    }

    public enum MessageColour
    {
        Background,
        Unimportant,
        Error,
        StateChangeSuccess,
        RecoverableFailure,
        Input,
        Text,
        Impure,
        StateLog
    }

    public static class MessageColorEx
    {
        public static ANSIColour ToANSIColour(this MessageColour @this, ColourScheme scheme = ColourScheme.TerminalBlack)
        {
            switch (scheme)
            {
                case ColourScheme.TerminalBlack:
                    switch (@this)
                    {
                        case MessageColour.StateLog:
                            return ANSIColour.Cyan;
                        case MessageColour.Background:
                            return ANSIColour.Black;
                        case MessageColour.Error:
                            return ANSIColour.Red;
                        case MessageColour.RecoverableFailure:
                            return ANSIColour.DarkRed;
                        case MessageColour.Text:
                            return ANSIColour.White;
                        case MessageColour.StateChangeSuccess:
                            return ANSIColour.Green;
                        case MessageColour.Unimportant:
                            return ANSIColour.DarkGray;
                        case MessageColour.Impure:
                            return ANSIColour.Magenta;
                        case MessageColour.Input:
                            return ANSIColour.White;

                        default:
                            throw new ArgumentException("No mapping for: " + @this + " in the scheme: " + scheme, "this");
                    }

                default:
                    throw new ArgumentException("Color scheme: " + scheme + " is not implemented!", "scheme");
            }
        }
    }
}
