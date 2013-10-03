using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32ConEcho;

namespace UnicodeConsole.Infrastructure.Shell
{
    class LineBuffer
    {
        private readonly StringBuilder _buffer;
        private readonly List<int> _wordBoundaries;
        private ColourScheme _scheme;
        private readonly ColourPair _defaultColour;

        public LineBuffer(ColourScheme scheme, int capacity)
        {
            _scheme = scheme;
            _buffer = new StringBuilder(capacity);
            _wordBoundaries = new List<int>(5);
            _defaultColour = new ColourPair(MessageColour.Text.ToANSIColour(_scheme), MessageColour.Background.ToANSIColour(_scheme));
        }

        internal void Append(char keyChar)
        {
            if (char.IsSeparator(keyChar))
            {
                _wordBoundaries.Add(_buffer.Length);
            }

            _buffer.Append(keyChar);
        }

        internal void RemoveEndChars(uint toBeRemoved = 1)
        {
            var charsToBeRemoved = Math.Min(_buffer.Length, toBeRemoved).EnsureInt32();
            if (charsToBeRemoved > 0)
            {
                var toBeRemovedStart = _buffer.Length - charsToBeRemoved;
                _buffer.Remove(toBeRemovedStart, charsToBeRemoved);

                while (_wordBoundaries.Count > 0 && _wordBoundaries.Last() > toBeRemovedStart)
                {
                    _wordBoundaries.RemoveAt(_wordBoundaries.Count - 1);
                }
            }
        }

        internal void Clear()
        {
            _buffer.Clear();
            _wordBoundaries.Clear();
        }

        internal string RawInput()
        {
            return _buffer.ToString();
        }

        /// <summary>
        /// Returns the colourised buffer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _buffer.ToString();
        }

        internal string LastWord()
        {
            var lastBoundaryCount = _wordBoundaries.Count;
            if (lastBoundaryCount > 0)
            {
                var lastBoundary = _wordBoundaries[lastBoundaryCount - 1];
                var beginOfLastWord = lastBoundary + 1;
                return _buffer.ToString(beginOfLastWord, _buffer.Length - beginOfLastWord);
            }
            else
            {
                return _buffer.ToString();
            }
        }

        internal ColourPair LastWordColours(out string lastWord)
        {
            lastWord = LastWord();
            if (lastWord == "if")
            {
                return new ColourPair(ANSIColour.Blue, _defaultColour.Background);
            }

            return _defaultColour;
            
        }
    }
}
