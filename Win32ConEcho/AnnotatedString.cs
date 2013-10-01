using System;
using System.Collections.Generic;
using System.Text;

namespace Win32ConEcho
{
    internal struct AnnotatedString
    {
        public readonly Annotation Annotation;
        public readonly string Text;

        public AnnotatedString(Annotation annotation, string text)
        {
            Annotation = annotation;
            Text = text;
        }

        public void ExecuteConsoleCommand()
        {
            switch (Annotation)
            {
                case Annotation.Text:
                    Console.Write(Text);
                    break;

                case Annotation.ColorEscape:
                    var switches = string.IsNullOrEmpty(Text) ? new []{"0"} : Text.Split(';');
                    var usesText = switches.Length >= 1 && char.IsLetter(switches[0], 0);
                    if (usesText)
                    {
                        Console.ForegroundColor = ANSIColor.ParseANSIColor(switches[0]);
                        if (switches.Length >= 2)
                            Console.BackgroundColor = ANSIColor.ParseANSIColor(switches[1]);
                    }
                    else
                    {
                        var colorDirective = ANSIColor.ParseANSIColorDirective(switches[0]);
                        var brightColors = colorDirective == 1;
                        switch (switches.Length)
                        {
                            case 1:
                                switch (colorDirective)
                                {
                                    case 0:
                                        Console.ResetColor();
                                        break;
                                }
                                break;

                            case 2:
                                Console.ForegroundColor = ANSIColor.ParseANSIColor(switches[1], 30, brightColors);
                                break;

                            case 3:
                                Console.BackgroundColor = ANSIColor.ParseANSIColor(switches[2], 40, brightColors);
                                goto case 2;
                        }
                    }
                    break;
            }
        }

        public static IEnumerable<AnnotatedString> AnnotateText(string input)
        {
            var length = input.Length;
            var buffer = new StringBuilder(length);
            var cursor = 0;
            const Annotation annotation = Annotation.Text;
            while (cursor < length)
            {
                var c = input[cursor++];

                switch (c)
                {
                    case '\\':
                        if (cursor == length)
                            throw new ArgumentException(input + " has an invalid escape sequence at=" + cursor, "input");

                        var next = input[cursor++];
                        switch (next)
                        {
                            case '"':
                            case '\\':
                                buffer.Append(next);
                                break;

                            case 'n':
                                buffer.Append('\n');
                                break;

                            case 't':
                                buffer.Append('\t');
                                break;

                            case 'e':
                                yield return new AnnotatedString(annotation, buffer.ToString());
                                buffer.Clear();

                                var seqEndIndex = input.IndexOfAny(new[] { (char)Annotation.ColorEscape, '_', ' ' }, cursor);

                                Annotation sequenceType;
                                string escapeSequenceParams;
                                var eatSequenceEndMarker = false;
                                if (cursor == length)
                                {
                                    sequenceType = Annotation.ColorEscape;
                                    escapeSequenceParams = string.Empty;
                                }
                                else if (seqEndIndex < cursor)
                                {
                                    throw new ArgumentException(
                                        string.Format("{0} has an unrecognized ANSI escape sequence at={1}", input,
                                                      cursor), "input");
                                }
                                else
                                {
                                    var sequenceEndMarker = input[seqEndIndex];
                                    eatSequenceEndMarker = sequenceEndMarker != ' ';
                                    sequenceType = ParseANSIEscapeSequenceMarker(sequenceEndMarker);
                                    escapeSequenceParams = input.Substring(cursor, seqEndIndex - cursor);
                                }

                                yield return new AnnotatedString(sequenceType, escapeSequenceParams);
                                
                                if (eatSequenceEndMarker)
                                    cursor = seqEndIndex + 1;
                                break;

                            default:
                                throw new ArgumentException(
                                    string.Format("{0} has an unrecognized escape char {1} at={2}", input, next, cursor),
                                    "input");
                        }
                        break;

                    default:
                        buffer.Append(c);
                        break;
                }
            }

            yield return new AnnotatedString(annotation, buffer.ToString());
        }

        private static Annotation ParseANSIEscapeSequenceMarker(char seqEnd)
        {
            switch (seqEnd)
            {
                case '_':
                case ' ':
                case (char)Annotation.ColorEscape: return Annotation.ColorEscape;

                default:
                    throw new ArgumentException(seqEnd + " is an unrecognized ANSI escape sequence");
            }
        }
    }
}