using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeConsole.FontProperties;

namespace UnicodeConsole.Infrastructure
{
    class Installer : DisposableBase
    {
        const string WindowsNT = @"Software\Microsoft\Windows NT\CurrentVersion";
        const string FontLinkPath = WindowsNT + @"\FontLink\SystemLink";

        public async Task Execute()
        {
            await LinkFontsAsync();
        }

        private async Task LinkFontsAsync()
        {
            Exception executingException = null;
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
                executingException = exception;
            }

            if (executingException != null)
            {
                await Console.Error.WriteLineAsync(executingException.Message);
                await Console.Error.WriteLineAsync("Testing:");
                await Console.Error.WriteLineAsync(" - Korean:  안녕하세요");
                await Console.Error.WriteLineAsync(" - Chinese: 你好！");
            }
        }

        protected override void DisposeUnmanagedMembers()
        {
            
        }

        protected override void DisposeManagedMembers()
        {
            
        }
    }
}
