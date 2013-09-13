using System.Text;
using System;
using UnicodeConsole.Infrastructure;

namespace UnicodeConsole
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var unicodeEncoding = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
            Console.InputEncoding = unicodeEncoding;
            Console.OutputEncoding = unicodeEncoding;

            Options options;
            if (args.Length > 0)
            {
                if (args[0].Length < 2)
                    throw new ArgumentException("Argument 0 (" + args[0] + ") is not long enough (>= 2)!", "args");

                switch (args[0][0])
                {
                    case '-':
                    case '/':
                        options = Options.ParseFirstOption(args[0].Substring(1));
                        break;

                    default:
                        throw new ArgumentException("Argument 0 (" + args[0] + ") is not a command option!", "args");
                }
            }
            else
            {
                options = Options.Default;
            }

            switch (options.StartupCommand)
            {
                case Options.Command.Install:
                    new Installer().Execute();
                    break;
            }

            if (options.StartCLI)
            {
                ConsoleKeyInfo key;
                while ((key = Console.ReadKey(true)).Key != ConsoleKey.Escape)
                {
                    Console.Write(key.KeyChar);
                }
            }
        }
    }
}
