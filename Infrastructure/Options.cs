using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure
{
    class Options
    {
        public enum Command
        {
            None = 0,
            Install
        }

        public static readonly Options Default = new Options(null);

        private readonly bool _startCLI;

        private readonly Command _command;

        private Options(string firstOption) {
            if (string.IsNullOrEmpty(firstOption))
            {
                _startCLI = true;
            }
            else
            {
                if (!Enum.TryParse(firstOption, true, out _command))
                {
                    throw new ArgumentException("Cannot recognize option: (" + firstOption + ")", "firstOption");
                }
            }
        }

        public static Options ParseFirstOption(string firstOption)
        {
            return new Options(firstOption);
        }

        public bool StartCLI { get { return _startCLI; } }

        public Command StartupCommand { get { return _command; } }
    }
}
