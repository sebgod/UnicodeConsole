using System;
using System.Collections.Generic;

namespace Win32ConEcho
{
    static class ANSIColor
    {
        private static readonly ConsoleColor[] ANSIBrightColors = new[]
            {
                ConsoleColor.Black,
                ConsoleColor.Red,   
                ConsoleColor.Green,  
                ConsoleColor.Yellow,
                ConsoleColor.Blue, 
                ConsoleColor.Magenta,
                ConsoleColor.Cyan,
                ConsoleColor.White,
                ConsoleColor.Gray
            };

        private static readonly ConsoleColor[] ANSIDarkColors = new[]
            {
                ConsoleColor.Black,
                ConsoleColor.DarkRed,   
                ConsoleColor.DarkGreen,  
                ConsoleColor.DarkYellow,
                ConsoleColor.DarkBlue, 
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkCyan,
                ConsoleColor.White,
                ConsoleColor.DarkGray
            };

        private static readonly Dictionary<string, ConsoleColor> CodeColorMapping = new Dictionary<string, ConsoleColor>
            {
                {"B", ConsoleColor.Black},
                {"r", ConsoleColor.Red},
                {"g", ConsoleColor.Green},
                {"y", ConsoleColor.Yellow},
                {"b", ConsoleColor.Blue},
                {"M", ConsoleColor.Magenta},
                {"c", ConsoleColor.Cyan},
                {"w", ConsoleColor.White},
                {"G", ConsoleColor.Gray},
                {"dr", ConsoleColor.DarkRed},
                {"dg", ConsoleColor.DarkGreen},
                {"dy", ConsoleColor.DarkYellow},
                {"db", ConsoleColor.DarkBlue},
                {"dM", ConsoleColor.DarkMagenta},
                {"dc", ConsoleColor.DarkCyan},
                {"dG", ConsoleColor.DarkGray},
            };

        public static ConsoleColor ParseANSIColor(string colorName)
        {
            ConsoleColor consoleColor;
            if (CodeColorMapping.TryGetValue(colorName, out consoleColor)
                || Enum.TryParse(colorName, true, out consoleColor)
                )
            {
                return consoleColor;
            }

            throw new ArgumentException(colorName + " is not a valid console color", "colorName");
        }

        public static ConsoleColor ParseANSIColor(string colorCode, int offset, bool intense)
        {
            var array = intense ? ANSIBrightColors : ANSIDarkColors;

            int colorValue;
            if (!Int32.TryParse(colorCode, out colorValue))
                throw new ArgumentException("Cannot parse the ANSI forground color: " + colorCode, "colorCode");

            int max = offset + array.Length;
            if (colorValue < offset || colorValue > max)
                throw new ArgumentOutOfRangeException("colorCode", colorValue,
                                                      String.Format("Color value should be in [{0},{1}]", offset, max));

            return array[colorValue - offset];
        }

        public static int ParseANSIColorDirective(string colorDirective)
        {
            int colorDirectiveValue;
            if (!int.TryParse(colorDirective, out colorDirectiveValue))
                throw new ArgumentException(colorDirective + " is unknown!", "colorDirective");

            return colorDirectiveValue;
        }
    }
}