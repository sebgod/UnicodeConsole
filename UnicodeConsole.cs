using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System;
using System.Drawing;
using UnicodeConsole.WinAPI;
using UnicodeConsole.FontProperties;
using System.Drawing.Text;

namespace UnicodeConsole
{
    public class Example
    {
        [STAThread]
        public static void Main()
        {
            var unicodeEncoding = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
            Console.InputEncoding = unicodeEncoding;
            Console.OutputEncoding = unicodeEncoding;

            RegistryKey fontLinkKey;
            try
            {
                fontLinkKey = Registry.LocalMachine.OpenSubKey(
                       @"Software\Microsoft\Windows NT\CurrentVersion\FontLink\SystemLink",
                       true);

            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                fontLinkKey = null;
            }

            Console.WriteLine("Listing fixed fonts:");
            var installedFixedFonts = new List<Font>();
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                installedFixedFonts.AddRange(from fontFamily in FontFamily.Families
                                             where fontFamily.IsStyleAvailable(FontStyle.Regular)
                                             let font = new Font(fontFamily, 11, FontStyle.Regular)
                                             where font.IsMonospaced(graphics)
                                             select font);
            }

            if (fontLinkKey == null)
            {
                Console.WriteLine("Font linking is not enabled. (or not running as administrator)");
                Console.WriteLine("안녕하세요");
                Console.WriteLine("你好！");

                Console.ReadKey();
            }
            else
            {
                var baseFonts = new FixedFontList { "consola.ttf,Consolas" };
                var toBeLinkedFonts = new FixedFontList { "simsun.ttc,SimSun" };

                var crossProduct =
                    from baseFont in baseFonts
                    from toBeLinkedFont in toBeLinkedFonts
                    select new { baseFont, toBeLinkedFont };

                foreach (var pair in crossProduct)
                {
                    ApplyFontLink(fontLinkKey, pair.baseFont, pair.toBeLinkedFont);
                }
            }

            if (fontLinkKey != null) fontLinkKey.Close();
        }

        private static void ApplyFontLink(RegistryKey fontLinkKey, FixedFont baseFont, FixedFont toBeLinkedFont)
        {
            FixedFontList existingLinkedFonts;
            RegistryValueKind kind = 0;
            bool toAdd;

            // Determine if the font is a base font. 
            var names = fontLinkKey.GetValueNames();
            var valueName = baseFont.Name;
            if (Array.Exists(names, s => s.Equals(valueName,
                                         StringComparison.OrdinalIgnoreCase)))
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
                    Console.WriteLine("Font is already linked.");
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
                Console.WriteLine("SimSun added to the list of linked fonts.");
            }
        }
    }

}
