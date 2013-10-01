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