using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeConsole.FontProperties;

namespace UnicodeConsole.Infrastructure
{
    class Installer
    {
        const string WindowsNT = @"Software\Microsoft\Windows NT\CurrentVersion";
        const string FontLinkPath = WindowsNT + @"\FontLink\SystemLink";

        public void Execute()
        {
            LinkFonts();    
        }

        private void LinkFonts()
        {
            try
            {
                using (var fontLinkKey = Registry.LocalMachine.OpenSubKey(FontLinkPath, true))
                {
                    var baseFonts = new FixedFontList {
                        "consola.ttf,Consolas", 
                        "lucon.ttf,Lucida Console"
                    };
                    var toBeLinkedFonts = new FixedFontList {
                        "simsun.ttc,SimSun",        // Chinese
                        "gulim.tcc,GuilimChe"       // Korean
                    };

                    baseFonts.LinkToFonts(fontLinkKey, toBeLinkedFonts);
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                Console.Error.WriteLine("Testing:");
                Console.Error.WriteLine(" - Korean:  안녕하세요");
                Console.Error.WriteLine(" - Chinese: 你好！");
            }
        }
    }
}
