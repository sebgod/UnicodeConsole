using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole.FontProperties
{
    public class FixedFontList : IReadOnlyCollection<FixedFont>
    {
        private readonly Dictionary<int, FixedFont> _linkedFonts;

        public FixedFontList(params string[] fonts)
        {
            _linkedFonts = new Dictionary<int, FixedFont>(fonts.Length);

            if (fonts != null)
            {
                foreach (var font in fonts)
                {
                    Add(font);
                }
            }
        }

        public void Add(FixedFont font)
        {
            if (Contains(font))
                return;

            var index = _linkedFonts.Count;
            _linkedFonts[index] = font;
        }

        public int Count { get { return _linkedFonts.Count; } }

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

        public void LinkToFonts(RegistryKey fontLinkKey, FixedFontList toBeLinkedFonts)
        {
            for (var i = 0; i < Count; i++)
            {
                var baseFont = _linkedFonts[i];

                for (var j = 0; j < toBeLinkedFonts.Count; j++)
                {
                    var toBeLinkedFont = toBeLinkedFonts._linkedFonts[j];
                    LinkFonts(fontLinkKey, ref baseFont, ref toBeLinkedFont);
                }
            }
        }

        private static void LinkFonts(RegistryKey fontLinkKey, ref FixedFont baseFont, ref FixedFont toBeLinkedFont)
        {

            FixedFontList existingLinkedFonts;
            RegistryValueKind kind = 0;
            bool toAdd;

            // Determine if the font is a base font. 
            var names = fontLinkKey.GetValueNames();
            var valueName = baseFont.Name;
            if (Array.Exists(names, s => s.Equals(valueName, StringComparison.OrdinalIgnoreCase)))
            {
                // Get the value's type.
                kind = fontLinkKey.GetValueKind(valueName);

                // Type should be RegistryValueKind.MultiString, but we can't be sure. 
                switch (kind)
                {
                    case RegistryValueKind.String:
                        existingLinkedFonts = new FixedFontList((string)fontLinkKey.GetValue(valueName));
                        break;
                    case RegistryValueKind.MultiString:
                        existingLinkedFonts = new FixedFontList((string[])fontLinkKey.GetValue(valueName));
                        break;

                    default:
                        // Do nothing.
                        existingLinkedFonts = new FixedFontList();
                        break;
                }

                if (existingLinkedFonts.Contains(toBeLinkedFont))
                {
                    Console.Error.WriteLine("{1} is already linked to {0}", baseFont, toBeLinkedFont);
                    toAdd = false;
                }
                else
                {
                    // Font is not a linked font.
                    toAdd = true;
                }
            }
            else
            {
                // Font is not a base font.
                toAdd = true;
                existingLinkedFonts = new FixedFontList();
            }

            if (toAdd)
            {
                existingLinkedFonts.Add(toBeLinkedFont);
                // Change REG_SZ to REG_MULTI_SZ. 
                if (kind == RegistryValueKind.String)
                    fontLinkKey.DeleteValue(valueName, false);

                fontLinkKey.SetValue(valueName, existingLinkedFonts.ToRegArray(), RegistryValueKind.MultiString);
                Console.WriteLine("Linked {1} to {0}", baseFont, toBeLinkedFont);
            }
        }

        public static IList<Font> InstalledFixedFonts
        {
            get
            {
                var installedFixedFonts = new List<Font>(30);
                using (Bitmap bitmap = new Bitmap(1, 1))
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    installedFixedFonts.AddRange(from fontFamily in FontFamily.Families
                                                 where fontFamily.IsStyleAvailable(FontStyle.Regular)
                                                 let font = new Font(fontFamily, 11, FontStyle.Regular)
                                                 where font.IsMonospaced(graphics)
                                                 select font);
                }

                return installedFixedFonts;
            }
        }
    }
}
