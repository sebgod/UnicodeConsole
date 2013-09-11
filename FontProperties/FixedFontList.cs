using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole
{
    public class FixedFontList : IEnumerable<FixedFont>
    {
        private readonly Dictionary<int, FixedFont> _linkedFonts;

        public FixedFontList(params string[] fonts)
        {
            _linkedFonts = new Dictionary<int, FixedFont>(fonts.Length);
        }

        public void Add(FixedFont font)
        {
            if (Contains(font))
                return;

            var index = _linkedFonts.Count;
            _linkedFonts[index] = font;
        }

        private bool Contains(FixedFont fontToFind)
        {
            return _linkedFonts.Any(linkedFont => linkedFont.Value == fontToFind);
        }

        public IEnumerator<FixedFont> GetEnumerator()
        {
            return _linkedFonts.Select(font => font.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal string[] ToRegArray()
        {
            var count = _linkedFonts.Count;
            var regArray = new string[count];

            for (var i = 0; i < count; i++)
            {
                regArray[i] = _linkedFonts[i].ToString();
            }

            return regArray;
        }
    }
}
