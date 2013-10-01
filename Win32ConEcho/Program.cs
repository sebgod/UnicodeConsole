using System;
using System.Collections.Generic;

namespace Win32ConEcho
{
    static class Program
    {
        static void Main(string[] args)
        {
            CommandSwitch switches;
            var index = ParseSwitches(args, out switches);

            WriteSentence(string.Join(" ", args, index, args.Length - index), switches);
        }

        private static int ParseSwitches(IList<string> args, out CommandSwitch switches)
        {
            switches = CommandSwitch.None;
            var index = 0;
            while (index < args.Count)
            {
                var possibleSwitch = args[index].ToLowerInvariant();
                if (possibleSwitch.Length == 0 || possibleSwitch[0] != '-')
                    break;

                switch (possibleSwitch)
                {
                    case "--":
                        return index+1;

                    case "-n":
                        switches |= CommandSwitch.NewLine;
                        break;

                    case "-t":
                        switches |= CommandSwitch.Tab;
                        break;

                    case "-x":
                        switches |= CommandSwitch.ExtendedSyntax;
                        break;

                    default:
                        throw new ArgumentException("Cannot parse option: " + possibleSwitch, "args");
                }
                index++;
            }
            return index;
        }

        private static void WriteSentence(string sentence, CommandSwitch switches)
        {
            if (switches.HasFlag(CommandSwitch.Tab))
                Console.Write('\t');

            if (switches.HasFlag(CommandSwitch.ExtendedSyntax))
            {
                foreach (var annotated in AnnotatedString.AnnotateText(sentence))
                    annotated.ExecuteConsoleCommand();
            }
            else
            {
                Console.Write(sentence);
            }

            if (switches.HasFlag(CommandSwitch.NewLine))
                Console.WriteLine();
        }

#if NET45
        private static async Task WriteSentenceAsync(string sentence, CommandSwitch switches)
        {
            if (switches.HasFlag(CommandSwitch.Tab))
                Console.Write('\t');

            if (switches.HasFlag(CommandSwitch.ExtendedSyntax))
            {
                foreach (var annotated in AnnotatedString.AnnotateText(sentence))
                    annotated.ExecuteConsoleCommand();
            }
            else
            {
                Console.Write(sentence);
            }

            if (switches.HasFlag(CommandSwitch.NewLine))
                Console.WriteLine();
        }
#endif
    }
}
