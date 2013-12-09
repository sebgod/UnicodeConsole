using System;
using System.Collections.Generic;
using System.Text;
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
                        if (switches.HasFlag(CommandSwitch.OnlyEscape))
                            throw new ArgumentException("Conflict: " + CommandSwitch.ExtendedSyntax + " already " + CommandSwitch.OnlyEscape + " set!", "args");
                        break;

                    case "-e":
                        switches |= CommandSwitch.OnlyEscape;
                        if (switches.HasFlag(CommandSwitch.ExtendedSyntax))
                            throw new ArgumentException("Conflict: " + CommandSwitch.OnlyEscape + " already " + CommandSwitch.ExtendedSyntax + " set!", "args");
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
                await Console.Out.WriteAsync('\t');

            if (commandSwitches.HasFlag(CommandSwitch.ExtendedSyntax))
            {
                foreach (var colouredString in sentence.Atomize().ToColourizedStrings())
                {
                    if (colouredString.ControlChar > '\0')
                    {
                        switch (colouredString.ControlChar)
                        {
                            case 'a':
                                ControlCharHelper.DoBeep(colouredString);
                                break;
                        }
                    }
                    else
                    {
                        if (colouredString.Colours.IsReset)
                        {
                            Console.ResetColor();
                        }
                        else if (colouredString.Colours.IsSwap)
                        {
                            var temp = Console.ForegroundColor;
                            Console.ForegroundColor = Console.BackgroundColor;
                            Console.BackgroundColor = temp;
                        }
                        else
                        {
                            if (colouredString.Colours.Foreground >= 0)
                            {
                                Console.ForegroundColor = (ConsoleColor)colouredString.Colours.Foreground;
                            }
                            if (colouredString.Colours.Background >= 0)
                            {
                                Console.BackgroundColor = (ConsoleColor)colouredString.Colours.Background;
                            }
                        }
                        await Console.Out.WriteAsync(colouredString.Text);
                    }
                }
            }
            else if (commandSwitches.HasFlag(CommandSwitch.OnlyEscape))
            {
                var inputLength = sentence.Length;
                var replacedChars = new StringBuilder(inputLength);
                for (var i = 0; i < inputLength; i++)
                {
                    var @char = sentence[i];
                    switch (@char)
                    {
                        case '\\':
                            char composed;
                            switch (sentence[++i])
                            {
                                case 'a': composed = '\a'; break;
                                case 'b': composed = '\b'; break;
                                case 'e': composed = '\x1b'; break;
                                case 'f': composed = '\f'; break;
                                case 'n': composed = '\n'; break;
                                case 'v': composed = '\v'; break;
                                default:
                                    throw new ArgumentException(@"Escape sequence \" + sentence[i] + " is not supported!", "sentence");
                            }
                            replacedChars.Append(composed);
                            break;

                        default:
                            replacedChars.Append(@char);
                            break;
                    }
                }
                await Console.Out.WriteAsync(replacedChars.ToString());
            }
            else
            {
                await Console.Out.WriteAsync(sentence);
            }

            if (commandSwitches.HasFlag(CommandSwitch.NewLine))
                await Console.Out.WriteLineAsync();
        }

    }
}
