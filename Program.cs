using System.Text;
using System;
using UnicodeConsole.Infrastructure;
using UnicodeConsole.Infrastructure.Shell;
using System.Threading.Tasks;

namespace UnicodeConsole
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var unicodeEncoding    = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
            Console.InputEncoding  = unicodeEncoding;
            Console.OutputEncoding = unicodeEncoding;
            // preventing a deadlock http://blogs.microsoft.co.il/blogs/dorony/archive/2012/09/12/console-readkey-net-4-5-changes-may-deadlock-your-system.aspx
            Console.Out.Flush();
            Console.Error.Flush(); 

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
                    using (var installer = new Installer())
                    {
                        Task.WaitAll(installer.Execute());
                    }
                    break;
            }

            if (options.StartCLI)
            {
                var shell = new ConsoleShell();
                Task.WaitAll(shell.ReadConsole());
            }
        }
    }
}
