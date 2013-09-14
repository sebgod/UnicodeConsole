using System;

namespace UnicodeConsole.Infrastructure.Shell
{
    public struct ConsoleHotkey
    {
        public readonly ConsoleModifiers Modifiers;
        public readonly ConsoleKey Key;

        public ConsoleHotkey(ConsoleModifiers modifiers, ConsoleKey key)
        {
            Modifiers = modifiers;
            Key = key;
        }

        public override string ToString()
        {
            return Modifiers.ToHotkeyString() + Key;
        }
    }
}