using System;

namespace Win32ConEcho
{
    [Flags]
    enum CommandSwitch
    {
        None = 0,
        ExtendedSyntax = 1,
        NewLine = 2,
        Tab = 4,
        OnlyEscape = 5,
    }
}