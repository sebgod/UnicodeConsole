using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure.Shell
{
    public static class ConsoleHelper
    {
        public static readonly AutoResetEvent ShutdownSignal = new AutoResetEvent(false);

        public static void GracefullCtrlC()
        {
            Console.CancelKeyPress += OnConsoleOnCancelKeyPress;
        }

        private static void OnConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            switch (e.SpecialKey)
            {
                case ConsoleSpecialKey.ControlBreak:
                    ShutdownSignal.Set();
                    break;

                case ConsoleSpecialKey.ControlC:
                    e.Cancel = true; // tell the CLR to keep running
                    ShutdownSignal.Set();
                    break;
            }
        }

        public static string ToHotkeyString(this ConsoleModifiers @this)
        {
            var builder = new StringBuilder(20);
            if ((@this & ConsoleModifiers.Control) != 0) builder.Append("CTRL+");
            if ((@this & ConsoleModifiers.Alt) != 0) builder.Append("ALT+");
            if ((@this & ConsoleModifiers.Shift) != 0) builder.Append("SHIFT+");

            return builder.ToString();
        }

    }
}
