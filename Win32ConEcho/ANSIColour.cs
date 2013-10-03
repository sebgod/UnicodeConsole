using System;
using System.Collections.Generic;

namespace Win32ConEcho
{
    public enum ANSIColour : sbyte
    {
        Reset = -2,
        Unchanged = -1,
        Black = ConsoleColor.Black, // = 0
        DarkBlue = ConsoleColor.DarkBlue, // = 1,
        DarkGreen = ConsoleColor.DarkGreen, // 2
        DarkCyan = ConsoleColor.DarkCyan, // 3
        DarkRed = ConsoleColor.DarkRed, // 4
        DarkMagenta = ConsoleColor.DarkMagenta, //5
        DarkYellow = ConsoleColor.DarkYellow, // 6
        Gray = ConsoleColor.Gray, // 7
        DarkGray = ConsoleColor.DarkGray, // 8
        Blue = ConsoleColor.Blue, // 9
        Green = ConsoleColor.Green, //10
        Cyan = ConsoleColor.Cyan, // 11
        Red = ConsoleColor.Red, // 12
        Magenta = ConsoleColor.Magenta, // 13
        Yellow = ConsoleColor.Yellow, // 14
        White = ConsoleColor.White // 15
    }

    public struct ColourPair
    {
        public readonly ANSIColour Foreground;
        public readonly ANSIColour Background;

        public ColourPair(ANSIColour foreground, ANSIColour background)
        {
            Foreground = foreground;
            Background = background;
        }

        public bool AreValid
        {
            get
            {
                return Foreground != Background || Foreground < 0;
            }
        }

        public bool IsReset
        {
            get
            {
                return Foreground == Background && Foreground == ANSIColour.Reset;
            }
        }
    }

    public enum ANSIColourDirective : byte
    {
        NoModifiers = 0,
        BrightColours = 1,
    }

    public static class ANSIColourEx
    {
        private static readonly ANSIColour[] ANSIBrightColors = new[]
            {
                ANSIColour.Black,
                ANSIColour.Red,   
                ANSIColour.Green,  
                ANSIColour.Yellow,
                ANSIColour.Blue, 
                ANSIColour.Magenta,
                ANSIColour.Cyan,
                ANSIColour.White,
                ANSIColour.Gray
            };

        private static readonly ANSIColour[] ANSIDarkColors = new[]
            {
                ANSIColour.Black,
                ANSIColour.DarkRed,   
                ANSIColour.DarkGreen,  
                ANSIColour.DarkYellow,
                ANSIColour.DarkBlue, 
                ANSIColour.DarkMagenta,
                ANSIColour.DarkCyan,
                ANSIColour.White,
                ANSIColour.DarkGray
            };

        private static readonly Dictionary<string, ANSIColour> CodeColorMapping = new Dictionary<string, ANSIColour>
            {
                {"B", ANSIColour.Black},
                {"r", ANSIColour.Red},
                {"g", ANSIColour.Green},
                {"y", ANSIColour.Yellow},
                {"b", ANSIColour.Blue},
                {"M", ANSIColour.Magenta},
                {"c", ANSIColour.Cyan},
                {"w", ANSIColour.White},
                {"G", ANSIColour.Gray},
                {"dr", ANSIColour.DarkRed},
                {"dg", ANSIColour.DarkGreen},
                {"dy", ANSIColour.DarkYellow},
                {"db", ANSIColour.DarkBlue},
                {"dM", ANSIColour.DarkMagenta},
                {"dc", ANSIColour.DarkCyan},
                {"dG", ANSIColour.DarkGray},
            };

        public static ANSIColour ParseANSIColour(string colorName)
        {
            ANSIColour ansiColor;
            if (CodeColorMapping.TryGetValue(colorName, out ansiColor)
                || Enum.TryParse(colorName, true, out ansiColor)
                )
            {
                return ansiColor;
            }

            throw new ArgumentException(colorName + " is not a valid console color", "colorName");
        }

        public static ANSIColour ParseANSIColour(string colorCode, int offset, bool intense)
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

        public static ANSIColourDirective ParseANSIColourDirective(string colorDirective)
        {
            int colorDirectiveValue;
            if (!int.TryParse(colorDirective, out colorDirectiveValue))
                throw new ArgumentException(colorDirective + " is unknown!", "colorDirective");

            return (ANSIColourDirective)colorDirectiveValue;
        }
    }
}