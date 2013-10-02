using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32ConEcho
{
    public struct ColouredString
    {
        public readonly ANSIColour Foreground;
        public readonly ANSIColour Background;
        public readonly string Text;

        public ColouredString(string text, ANSIColour foreground = ANSIColour.Unchanged, ANSIColour background = ANSIColour.Unchanged)
        {
            if (foreground == background && foreground >= 0)
                throw new ArgumentException("Cannot set console foreground and background to the same colour (" + background + ")!", "background");

            Foreground = foreground;
            Background = background;
            Text = text;
        }
    }

    public static class ColouredStringEx {
        // AnnotatedString.AnnotateText(sentence)
        public static IEnumerable<ColouredString> ToColourizedStrings(this IEnumerable<AnnotatedStringAtom> atoms)
        {
            var foreground = ANSIColour.Unchanged;
            var background = ANSIColour.Unchanged;

            foreach (var atom in atoms)
            {
                switch (atom.AtomType)
                {
                    case AtomType.Text:
                        yield return new ColouredString(atom.Text, foreground, background);
                        break;

                    case AtomType.ColorEscape:
                        var switches = string.IsNullOrEmpty(atom.Text) ? new[] { "0" } : atom.Text.Split(';');
                        var usesText = switches.Length >= 1 && char.IsLetter(switches[0], 0);
                        if (usesText)
                        {
                            foreground = ANSIColourEx.ParseANSIColour(switches[0]);
                            if (switches.Length >= 2)
                                background = ANSIColourEx.ParseANSIColour(switches[1]);
                        }
                        else
                        {
                            var colorDirective = ANSIColourEx.ParseANSIColourDirective(switches[0]);
                            var brightColors = colorDirective == 1;
                            switch (switches.Length)
                            {
                                case 1:
                                    switch (colorDirective)
                                    {
                                        case 0:
                                            foreground = ANSIColour.Reset;
                                            background = ANSIColour.Reset;
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
    }
}
