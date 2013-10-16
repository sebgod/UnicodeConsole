using System;
using System.Collections.Generic;
using System.Text;

namespace Win32ConEcho
{
    public struct AnnotatedStringAtom
    {
        public readonly AtomType AtomType;
        public readonly string Text;

        public AnnotatedStringAtom(AtomType annotation, string text)
        {
            AtomType = annotation;
            Text = text;
        }
    }

    public static class AnnotatedStringAtomEx {
        public static IEnumerable<AnnotatedStringAtom> Atomize(this string input)
        {
            var length = input.Length;
            var buffer = new StringBuilder(length);
            var cursor = 0;
            const AtomType annotation = AtomType.Text;
            while (cursor < length)
            {
                var c = input[cursor++];

                switch (c)
                {
                    // matching the literal ASCII control chars to their escaped versions
                    case '\a':
                    case '\x1B':
                        cursor--;
                        goto case '\\';

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
                            
                            case 'u':
                                var unicodeEscapeLength = input.UnicodeCodePointEscapeLength(cursor);
                                buffer.Append(char.ConvertFromUtf32((int)Convert.ToUInt32(input.Substring(cursor, unicodeEscapeLength), 16)));
                                break;
                            
                            case '\a':
                                if (next == c)
                                    goto case 'a';
                                goto default;

                            case 'a':
                                yield return buffer.FlushBuffer(annotation);
                                yield return new AnnotatedStringAtom(AtomType.ControlChar, Convert.ToString(next));
                                break;

                            case 't':
                                buffer.Append('\t');
                                break;

                            case '\x1B':
                                if (next == c)
                                    goto case 'e';
                                goto default;

                            case 'e':
                                yield return buffer.FlushBuffer(annotation);

                                var seqEndIndex = input.IndexOfAny(new[] { (char)AtomType.ColorEscape, '_', ' ' }, cursor);

                                AtomType sequenceType;
                                string escapeSequenceParams;
                                var eatSequenceEndMarker = false;
                                if (cursor == length)
                                {
                                    sequenceType = AtomType.ColorEscape;
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

                                yield return new AnnotatedStringAtom(sequenceType, escapeSequenceParams);
                                
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

            yield return new AnnotatedStringAtom(annotation, buffer.ToString());
        }

        private static int UnicodeCodePointEscapeLength(this string @this, int offset)
        {
            var unicodeEscapeLength = 4;
            if (@this.Length >= offset + 8 && @this[offset] == '0' && @this[offset + 1] == '0')
            {
                unicodeEscapeLength = 8;
                for (var i = offset + 4; i < offset + 8; i++)
                {
                    if (!@this.IsHexDigit(i))
                    {
                        unicodeEscapeLength = 4;
                        break;
                    }
                }
            }
            return unicodeEscapeLength;
        }

        private static bool IsHexDigit(this string @this, int index)
        {
            switch (@this[index])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return true;

                default:
                    return false;
            }
        }

        private static AnnotatedStringAtom FlushBuffer(this StringBuilder buffer, AtomType annotation)
        {
            var stringified = buffer.ToString();
            buffer.Clear();
            return new AnnotatedStringAtom(annotation, stringified);
        }

        private static AtomType ParseANSIEscapeSequenceMarker(char seqEnd)
        {
            switch (seqEnd)
            {
                case '_':
                case ' ':
                case (char)AtomType.ColorEscape: return AtomType.ColorEscape;

                default:
                    throw new ArgumentException(seqEnd + " is an unrecognized ANSI escape sequence");
            }
        }

        
    }
}