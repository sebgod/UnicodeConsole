using System;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure.Shell
{
    public delegate Task<ConsoleDelegateResult> ConsoleCommandDelegate(string args);
    
    [Flags]
    public enum ConsoleDelegateResult
    {
        /* Flags */
        Ok = 0,
        ExitFlag = 1,
        InhibitPrompt = 2,

        /* Compound */
        ExitShell = InhibitPrompt | ExitFlag
    }
}