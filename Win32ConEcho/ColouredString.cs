using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32ConEcho
{
    public struct ColouredString
    {
        public readonly ColourPair Colours;
        public readonly char ControlChar;
        public readonly string Text;

        public ColouredString(string text, char controlChar, ColourPair colours)
        {
            if (!colours.AreValid)
                throw new ArgumentException("Cannot set console foreground and background to the same colour (" + colours.Background + ")!", "colours");

            Colours = colours;
            ControlChar = controlChar;
            Text = text;
        }
    }

    public enum UnderlineType
    {
        None = 0,
        Single = 1,
        Double = 2,
    }

    public static class ColouredStringEx {
        // AnnotatedString.AnnotateText(sentence)
        public static IEnumerable<ColouredString> ToColourizedStrings(this IEnumerable<AnnotatedStringAtom> atoms)
        {
            var foreground = ANSIColour.Unchanged;
            var background = ANSIColour.Unchanged;
            var underline = UnderlineType.None;

            foreach (var atom in atoms)
            {
                switch (atom.AtomType)
                {
                    case AtomType.Text:
                        yield return new ColouredString(ApplyUnderline(atom.Text, underline), '\0', new ColourPair(foreground, background));
                        break;

                    case AtomType.ControlChar:
                        yield return new ColouredString(null, atom.Text[0], new ColourPair(foreground, background));
                        break;

                    case AtomType.ColorEscape:
                        var switches = string.IsNullOrEmpty(atom.Text) ? new[] { "0" } : atom.Text.Split(';');
                        var usesText = switches.Length >= 1 && ( char.IsLetter(switches[0], 0) || switches[0][0] == '+' );
                        if (usesText)
                        {
                            foreground = ANSIColourEx.ParseANSIColour(switches[0]);
                            if (switches.Length >= 2)
                                background = ANSIColourEx.ParseANSIColour(switches[1]);
                        }
                        else
                        {
                            var colorDirective = ANSIColourEx.ParseANSIColourDirective(switches[0]);
                            var brightColors = colorDirective == ANSIColourDirective.BrightColours;
                            switch (switches.Length)
                            {
                                case 1:
                                    switch (colorDirective)
                                    {
                                        case ANSIColourDirective.NoModifiers:
                                            foreground = ANSIColour.Reset;
                                            background = ANSIColour.Reset;
                                            underline = UnderlineType.None;
                                            break;

                                        case ANSIColourDirective.Inverse:
                                            foreground = ANSIColour.Swap;
                                            background = ANSIColour.Swap;
                                            break;

                                        case ANSIColourDirective.UnderlineSingle:
                                            underline = UnderlineType.Single;
                                            break;
                                    }
                                    break;

                                case 2:
                                    foreground = ANSIColourEx.ParseANSIColour(switches[1], 30, brightColors);
                                    break;

                                case 3:
                                    background = ANSIColourEx.ParseANSIColour(switches[2], 40, brightColors);
                                    goto case 2;
                            }
                        }
                        break;
                }
            }
        }

        private static string ApplyUnderline(string text, UnderlineType underline)
        {
            switch (underline)
            {
                case UnderlineType.None:
                    return text;

                case UnderlineType.Single:
                    var length = text.Length;
                    var underlined = new StringBuilder(length * 2 + 2);
                    for (var i = 0; i < length; i++)
                    {
                        var c = text[i];
                        underlined.Append(c);
                        if (char.IsSurrogate(c))
                        {
                            underlined.Append(text[i + 1]);
                            if (char.IsLetterOrDigit(text, i))
                            {
                                underlined.Append('\u0332');
                            }
                            i++;
                        }
                        else if (char.IsLetterOrDigit(c))
                        {
                            underlined.Append('\u0332');
                        }
                    }
                    return underlined.ToString();

                case UnderlineType.Double:
                default:
                    throw new ArgumentException(string.Format("UnderlineType \"{0}\" is not supported!", underline), "underline");
            }
        }
    }
}
