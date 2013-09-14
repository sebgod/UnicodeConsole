using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure.Shell
{
    public enum ColorScheme
    {
        TerminalBlack
    }

    public enum MessageColor
    {
        Background,
        Unimportant,
        Error,
        StateChangeSuccess,
        RecoverableFailure,
        Informative,
        Impure,
        Queuing
    }

    public static class MessageColorEx
    {
        public static ConsoleColor ToConsoleColor(this MessageColor @this,
                                                  ColorScheme scheme = ColorScheme.TerminalBlack)
        {
            switch (scheme)
            {
                case ColorScheme.TerminalBlack:
                    switch (@this)
                    {
                        case MessageColor.Queuing:
                            return ConsoleColor.Cyan;
                        case MessageColor.Background:
                            return ConsoleColor.Black;
                        case MessageColor.Error:
                            return ConsoleColor.Red;
                        case MessageColor.RecoverableFailure:
                            return ConsoleColor.DarkRed;
                        case MessageColor.Informative:
                            return ConsoleColor.White;
                        case MessageColor.StateChangeSuccess:
                            return ConsoleColor.Green;
                        case MessageColor.Unimportant:
                            return ConsoleColor.DarkGray;
                        case MessageColor.Impure:
                            return ConsoleColor.Magenta;

                        default:
                            throw new ArgumentException("No mapping for: " + @this + " in the scheme: " + scheme, "this");
                    }

                default:
                    throw new ArgumentException("Color scheme: " + scheme + " is not implemented!", "scheme");
            }
        }
    }
}
