using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32ConEcho
{
    public static class ControlCharHelper
    {
        public static void DoBeep(ColouredString colouredString)
        {
            int beepFreq;
            int beepDuration;
            if (TryParseBellParams(colouredString.Text, out beepFreq, out beepDuration))
                Console.Beep(beepFreq, beepDuration);
            else
                Console.Beep();
        }

        public static bool TryParseBellParams(string text, out int beepFreq, out int beepDuration)
        {
            beepFreq = 0;
            beepDuration = 0;
            if (string.IsNullOrEmpty(text))
                return false;

            var beepParams = text.Split(new[] { ';' });
            return beepParams.Length == 2 && int.TryParse(beepParams[0], out beepFreq) && int.TryParse(beepParams[1], out beepDuration);
        }
    }
}
