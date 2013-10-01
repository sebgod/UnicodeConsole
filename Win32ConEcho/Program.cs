using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Win32ConEcho
{
    static class Program
    {
        static void Main(string[] args)
        {
            var unicodeEncoding = new UnicodeEncoding(!BitConverter.IsLittleEndian, false);
            Console.InputEncoding = unicodeEncoding;
            Console.OutputEncoding = unicodeEncoding;
            // preventing a deadlock http://blogs.microsoft.co.il/blogs/dorony/archive/2012/09/12/console-readkey-net-4-5-changes-may-deadlock-your-system.aspx
            Console.Out.Flush();
            Console.Error.Flush(); 

            CommandSwitch switches;
            var index = ParseSwitches(args, out switches);

            Task.WaitAll(WriteSentenceAsync(string.Join(" ", args, index, args.Length - index), switches));
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

        private static async Task WriteSentenceAsync(string sentence, CommandSwitch commandSwitches)
        {
            if (commandSwitches.HasFlag(CommandSwitch.Tab))
                Console.Write('\t');

            if (commandSwitches.HasFlag(CommandSwitch.ExtendedSyntax))
            {
                foreach (var annotated in AnnotatedString.AnnotateText(sentence))
                {
                    switch (annotated.Annotation)
                    {
                        case Annotation.Text:
                            Console.Write(annotated.Text);
                            break;

                        case Annotation.ColorEscape:
                            var switches = string.IsNullOrEmpty(annotated.Text) ? new[] { "0" } : annotated.Text.Split(';');
                            var usesText = switches.Length >= 1 && char.IsLetter(switches[0], 0);
                            if (usesText)
                            {
                                Console.ForegroundColor = ANSIColor.ParseANSIColor(switches[0]);
                                if (switches.Length >= 2)
                                    Console.BackgroundColor = ANSIColor.ParseANSIColor(switches[1]);
                            }
                            else
                            {
                                var colorDirective = ANSIColor.ParseANSIColorDirective(switches[0]);
                                var brightColors = colorDirective == 1;
                                switch (switches.Length)
                                {
                                    case 1:
                                        switch (colorDirective)
                                        {
                                            case 0:
                                                Console.ResetColor();
                                                break;
                                        }
                                        break;

                                    case 2:
                                        Console.ForegroundColor = ANSIColor.ParseANSIColor(switches[1], 30, brightColors);
                                        break;

                                    case 3:
                                        Console.BackgroundColor = ANSIColor.ParseANSIColor(switches[2], 40, brightColors);
                                        goto case 2;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                Console.Write(sentence);
            }

            if (commandSwitches.HasFlag(CommandSwitch.NewLine))
                Console.Out.WriteLine();
        }
    }
}
