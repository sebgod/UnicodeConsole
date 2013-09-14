using System;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure.Shell
{
    public class ConsoleCommand : Tuple<string, string, ConsoleCommandDelegate, ConsoleHotkey?>
    {
        public ConsoleCommand(string name, string description, ConsoleCommandDelegate @delegate, ConsoleKey hotkey)
            : this(name, description, @delegate, new ConsoleHotkey(0, hotkey))
        {
            // calling ConsoleCommand(string name, string description, ConsoleHotkey? hotkey = null)
        }

        public ConsoleCommand(string name, string description, ConsoleCommandDelegate @delegate,
                              ConsoleHotkey? hotkey = null)
            : base(name, description, @delegate, hotkey)
        {

        }

        public string Name
        {
            get { return Item1; }
        }

        public string Desciption
        {
            get { return Item2; }
        }

        public ConsoleHotkey? Hotkey
        {
            get { return Item4; }
        }

        public string ToString(uint nameLength = 20, uint hotkeyLength = 15)
        {
            var nameAndHotkeyFormat = string.Format("{{0,-{0}}} {{1,-{1}}}", nameLength, hotkeyLength);
            return Desciption.PrefixedMultiLineString(Console.BufferWidth, nameAndHotkeyFormat, Name, string.Format("[{0}] ", Hotkey));
        }

        public bool CanApply(string commandName)
        {
            return string.Compare(commandName, Name, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public async Task<ConsoleDelegateResult> ApplyAsync(string args)
        {
            return await Item3(args);
        }
    }
}