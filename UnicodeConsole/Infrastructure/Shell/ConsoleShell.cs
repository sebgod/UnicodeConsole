using System;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Win32ConEcho;

namespace UnicodeConsole.Infrastructure.Shell
{
    public class ConsoleShell : DisposableBase
    {
        const string Prompt = ">";
        const ConsoleModifiers CntrlAltMask = ConsoleModifiers.Alt | ConsoleModifiers.Control;

        private readonly PowerShell _ps;
        private readonly ConsoleCommand[] _commandTable;
        private readonly LineBuffer _lineBuffer;

        public ConsoleShell()
        {
            _ps = PowerShell.Create();
            _lineBuffer = new LineBuffer(Console.BufferWidth);
            _commandTable = new[]
                {
                    new ConsoleCommand("exit", "Closes the console",
                                       async args =>
                                           {
                                               await WriteMessageAsync("Closing this terminal!", MessageColour.Warning);
                                               return ConsoleDelegateResult.ExitShell;
                                           },
                                       ConsoleKey.Escape),
                    new ConsoleCommand("help", "Shows this help", ShowUsage, ConsoleKey.F1),
                    
                    new ConsoleCommand("version", "Shows version information",
                                       async args =>
                                           {
                                               var psHost = (await AddCommandAsync("Get-Host")).First();
                                               var entryAssembly = Assembly.GetEntryAssembly();
                                               var versionMessage = string.Format("{0} {1} by {2} PowerShell v{3}", 
                                                   entryAssembly.FullName, "Copyright© 2013", "Sebastian Godelet <sebastian.godelet@gmail.com>",
                                                   psHost.Members["Version"].Value
                                               );
                                               await WriteMessageAsync(versionMessage, MessageColour.Text);
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
                MessageColour.Error).Wait(-1);
        }

        public async Task WritePromptAsync()
        {
            UpdateColours(MessageColour.Text);
            if (Console.CursorLeft == 0)
            {
                await Console.Out.WriteAsync(Prompt);
            }
        }

        public async Task WriteMessageAsync(string message, MessageColour messageColor)
        {
            Console.CursorLeft = 0;
            UpdateColours(messageColor);

            var outputStream = messageColor == MessageColour.Error ? Console.Error : Console.Out;
            await outputStream.WriteLineAsync(message);
            await WritePromptAsync();
            await outputStream.WriteAsync(_lineBuffer.ToString());
        }

        private void UpdateColours(MessageColour messageColor, MessageColour backgroundColor = MessageColour.Background)
        {
            UpdateColours(new ColourPair(messageColor.ToANSIColour(), backgroundColor.ToANSIColour()));
        }

        private void UpdateColours(ColourPair colourPair)
        {
            Console.BackgroundColor = (ConsoleColor)colourPair.Background;
            Console.ForegroundColor = (ConsoleColor)colourPair.Foreground;
        }

        public async Task<ConsoleDelegateResult> ReadConsole()
        {
            await WritePromptAsync();
            var commandResult = ConsoleDelegateResult.Ok;
            do
            {
                var keyInfo = Console.ReadKey(true);
                var consoleKey = keyInfo.Key;
                var keyChar = keyInfo.KeyChar;
                var consoleModifiers = keyInfo.Modifiers;

                var isShift = (consoleModifiers & ConsoleModifiers.Shift) != 0;
                var controlOrAlt = consoleModifiers & CntrlAltMask;

                switch (controlOrAlt)
                {
                    case CntrlAltMask:
                        await WriteMessageAsync(string.Format("Unhandled key: Cntrl{1}-Alt-{0}", consoleKey, isShift ? "-Shift" : ""), MessageColour.Error);
                        await Task.Delay(1000);
                        break;

                    case ConsoleModifiers.Control:
                        switch (consoleKey)
                        {
                            case ConsoleKey.LeftArrow:
                                Console.CursorLeft = Prompt.Length;
                                break;

                            case ConsoleKey.Backspace:
                                if (isShift)
                                    goto default;
                                break;

                            default:
                                await WriteMessageAsync(string.Format("Unhandled key: Cntrl-{1}{0}", consoleKey, isShift ? "-Shift" : ""), MessageColour.Error);
                                await Task.Delay(1000);
                                break;
                        }
                        break;

                    case ConsoleModifiers.Alt:
                        await WriteMessageAsync(string.Format("Pressed Alt-{0}", consoleKey), MessageColour.Text);
                        await Task.Delay(1000);
                        break;

                    case 0:
                        commandResult = await ProcessUnmodifiedConsoleKey(commandResult, consoleKey, keyChar);
                        break;
                }
            } while (!commandResult.HasFlag(ConsoleDelegateResult.ExitFlag));
            return commandResult;
        }

        private async Task<ConsoleDelegateResult> ProcessUnmodifiedConsoleKey(ConsoleDelegateResult commandResult, ConsoleKey consoleKey, char keyChar)
        {
            switch (consoleKey)
            {
                case ConsoleKey.Enter:
                    {
                        var input = _lineBuffer.RawInput();
                        _lineBuffer.Clear();
                        await Console.Out.WriteLineAsync();

                        var commandName = new Regex(@"^([^(\s]+)\b", RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant).Match(input).Groups[1].Value.Trim();
                        var args = input.Substring(commandName.Length).Trim();

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
                                    await WriteMessageAsync("Result: " + result, MessageColour.OK);
                                }
                            }
                        }
                        else
                        {
                            await WriteMessageAsync("multiple matching actions for: " + commandName, MessageColour.Error);
                        }

                        if (!commandResult.HasFlag(ConsoleDelegateResult.InhibitPrompt))
                            await WritePromptAsync();
                    }
                    break;

                case ConsoleKey.Backspace:
                    if (Console.CursorLeft > Prompt.Length)
                    {
                        Console.CursorLeft--;
                        await Console.Out.WriteAsync(' ');
                        Console.CursorLeft--;
                        _lineBuffer.RemoveEndChars();
                    }
                    break;
                default:
                    if (!char.IsControl(keyChar))
                    {
                        _lineBuffer.Append(keyChar);
                        string lastWord;
                        UpdateColours(_lineBuffer.LastWordColours(out lastWord));
                        // -1 because the currently typed char has not been written yet
                        Console.CursorLeft -= (lastWord.Length - 1); 
                        await Console.Out.WriteAsync(lastWord);
                    }
                    break;
            }
            return commandResult;
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
                await WriteMessageAsync(command.ToString(), MessageColour.Text);
            }

            return ConsoleDelegateResult.Ok;
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