using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure.Shell
{
    public class ConsoleShell
    {
        private readonly ConsoleCommand[] _commandTable;
        private readonly ColorScheme _scheme;
        private readonly StringBuilder _lineBuffer;

        public ConsoleShell(ColorScheme scheme = ColorScheme.TerminalBlack)
        {
            _lineBuffer = new StringBuilder(Console.BufferWidth);
            _scheme = scheme;
            _commandTable = new[]
                {
                    new ConsoleCommand("exit", "Closes the console",
                                       async args =>
                                           {
                                               return ConsoleDelegateResult.ExitShell;
                                           },
                                       ConsoleKey.Escape),
                    new ConsoleCommand("help", "Zeigt diese Hilfe an", ShowUsage, ConsoleKey.F1),
                    
                    new ConsoleCommand("ver", "Zeigt die Version der ausführenden Assembly an",
                                       async args =>
                                           {
                                               var entryAssembly = Assembly.GetEntryAssembly();
                                               var fileVersion = entryAssembly.GetCustomAttributes<AssemblyFileVersionAttribute>().SingleOrDefault();
                                               await WriteMessageAsync("Version: " + (fileVersion != null ? fileVersion.Version : "<unknown>"), MessageColor.Informative);
                                               return ConsoleDelegateResult.Ok;
                                           },
                                       new ConsoleHotkey(ConsoleModifiers.Alt, ConsoleKey.V)
                        ),

                };

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            ConsoleHelper.GracefullCtrlC();
        }

        private void CurrentDomainOnUnhandledException(object sender,
                                                       UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            WriteMessageAsync(string.Format("from: {0} exeption: {1}: terminating: {2}", sender, unhandledExceptionEventArgs.ExceptionObject, unhandledExceptionEventArgs.IsTerminating),
                MessageColor.Error).Wait(-1);
        }

        public async Task WritePromptAsync()
        {
            Console.BackgroundColor = MessageColor.Background.ToConsoleColor(_scheme);
            Console.ForegroundColor = MessageColor.Informative.ToConsoleColor(_scheme);
            if (Console.CursorLeft == 0)
            {
                await Console.Out.WriteAsync('>');
            }
        }

        public async Task ReadConsole()
        {
            await WritePromptAsync();
            var commandResult = ConsoleDelegateResult.Ok;
            do
            {
                var keyInfo = Console.ReadKey(true);
                var consoleKey = keyInfo.Key;
                var keyChar = keyInfo.KeyChar;
                var consoleModifiers = keyInfo.Modifiers;

                var alt = (consoleModifiers & ConsoleModifiers.Alt) != 0;
                var shift = (consoleModifiers & ConsoleModifiers.Shift) != 0;
                var cntrl = (consoleModifiers & ConsoleModifiers.Control) != 0;
                var isAltOrCntrl = alt || cntrl;

                if (isAltOrCntrl)
                {
                    // TODO: handle special commands
                    if (cntrl)
                    {
                        switch (consoleKey)
                        {
                            case ConsoleKey.LeftArrow:
                                Console.CursorLeft = 1;
                                break;
                        }
                    }
                }
                else
                {
                    switch (consoleKey)
                    {
                        case ConsoleKey.Enter:
                            {
                                var input = _lineBuffer.ToString();
                                _lineBuffer.Clear();
                                Console.WriteLine();

                                var commandName = new Regex(@"^(\w+)\b").Match(input).Groups[1].Value.Trim();
                                var args = input.Substring(commandName.Length).Trim();

                                Console.ForegroundColor = ConsoleColor.White;

                                Console.ResetColor();

                                var applicableCommands =
                                    (from commandInfo in _commandTable
                                     where commandInfo.CanApply(commandName)
                                     select commandInfo).ToList();

                                if (applicableCommands.Count == 1)
                                {
                                    commandResult = await applicableCommands[0].ApplyAsync(args);
                                }
                                else if (applicableCommands.Count == 0)
                                {
                                    if (commandName.Length > 0)
                                    {
                                        await WriteMessageAsync("cannot find an action for: " + commandName, MessageColor.Error);
                                    }
                                }
                                else
                                {
                                    await WriteMessageAsync("multiple matching actions for: " + commandName, MessageColor.Error);
                                }

                                if (!commandResult.HasFlag(ConsoleDelegateResult.InhibitPrompt))
                                    await WritePromptAsync();
                            }
                            break;
                        case ConsoleKey.Backspace:
                            if (Console.CursorLeft > 1)
                            {
                                Console.CursorLeft--;
                                Console.Write(' ');
                                Console.CursorLeft--;
                                if (_lineBuffer.Length > 0)
                                    _lineBuffer.Remove(_lineBuffer.Length - 1, 1);
                            }
                            break;
                        default:
                            if (!char.IsControl(keyChar))
                            {
                                _lineBuffer.Append(keyChar);
                                if (Console.CursorLeft == 0)
                                    await WritePromptAsync();

                                Console.Write(keyChar);
                            }
                            break;
                    }
                }
            } while (!commandResult.HasFlag(ConsoleDelegateResult.ExitFlag));
        }

        public async Task<ConsoleDelegateResult> ShowUsage(string args = null)
        {
            foreach (var command in _commandTable)
            {
                await WriteMessageAsync(command.ToString(), MessageColor.Informative);
            }

            return ConsoleDelegateResult.Ok;
        }


        public async Task WriteMessageAsync(string message, MessageColor messageColor)
        {
            Console.CursorLeft = 0;
            Console.ForegroundColor = messageColor.ToConsoleColor(_scheme);

            var outputStream = messageColor == MessageColor.Error ? Console.Error : Console.Out;
            await outputStream.WriteLineAsync(message);
            await WritePromptAsync();
            await outputStream.WriteAsync(_lineBuffer.ToString());
        }
    }
}