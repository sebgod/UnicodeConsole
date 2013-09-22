using System;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure.Shell
{
    public class ConsoleShell : DisposableBase
    {
        private readonly PowerShell _ps;
        private readonly ConsoleCommand[] _commandTable;
        private readonly ColorScheme _scheme;
        private readonly StringBuilder _lineBuffer;

        public ConsoleShell(ColorScheme scheme = ColorScheme.TerminalBlack)
        {
            _ps = PowerShell.Create();
            _lineBuffer = new StringBuilder(Console.BufferWidth);
            _scheme = scheme;
            _commandTable = new[]
                {
                    new ConsoleCommand("exit", "Closes the console",
                                       async args =>
                                           {
                                               await WriteMessageAsync("Bye bye!", MessageColor.Unimportant);
                                               return ConsoleDelegateResult.ExitShell;
                                           },
                                       ConsoleKey.Escape),
                    new ConsoleCommand("help", "Shows this help", ShowUsage, ConsoleKey.F1),
                    
                    new ConsoleCommand("ver", "Shows version information",
                                       async args =>
                                           {
                                               var psHost = (await AddCommandAsync("Get-Host")).First();
                                               var entryAssembly = Assembly.GetEntryAssembly();
                                               var versionMessage = string.Format("{0} {1} by {2} PowerShell v{3}", 
                                                   entryAssembly.FullName, "Copyright© 2013", "Sebastian Godelet <sebastian.godelet@gmail.com>",
                                                   psHost.Members["Version"].Value
                                               );
                                               await WriteMessageAsync(versionMessage, MessageColor.Text);
                                               return ConsoleDelegateResult.Ok;
                                           },
                                       null
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
            Console.ForegroundColor = MessageColor.Text.ToConsoleColor(_scheme);
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
                    if (cntrl && !alt)
                    {
                        switch (consoleKey)
                        {
                            case ConsoleKey.LeftArrow:
                                Console.CursorLeft = 1;
                                break;

                            default:
                                await WriteMessageAsync(string.Format("Pressed Cntrl-{0}", consoleKey), MessageColor.Text);
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

                                var commandName = new Regex(@"^([^(\s]+)\b", RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant).Match(input).Groups[1].Value.Trim();
                                var args = input.Substring(commandName.Length).Trim();

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
                                        var commandResults = await AddCommandAsync(commandName);
                                        foreach (var result in commandResults)
                                        {
                                            await WriteMessageAsync("Result: " + result, MessageColor.StateChangeSuccess);
                                        }
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

        private async Task<PSDataCollection<PSObject>> AddCommandAsync(string commandName)
        {
            _ps.AddCommand(commandName);
            return await Task.Factory.FromAsync(_ps.BeginInvoke(), pResult => _ps.EndInvoke(pResult));
        }

        public async Task<ConsoleDelegateResult> ShowUsage(string args = null)
        {
            foreach (var command in _commandTable)
            {
                await WriteMessageAsync(command.ToString(), MessageColor.Text);
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

        protected override void DisposeUnmanagedMembers()
        {

        }

        protected override void DisposeManagedMembers()
        {
            _ps.Dispose();
        }
    }
}