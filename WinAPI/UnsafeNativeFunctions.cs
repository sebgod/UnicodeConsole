using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnicodeConsole.FontProperties;

namespace UnicodeConsole.WinAPI
{
    static class UnsafeNativeFunctions
    {
        // if we specify CharSet.Auto instead of CharSet.Ansi, then the string will be unreadable
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            [MarshalAs(UnmanagedType.U4)]
            public FontWeight lfWeight;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfItalic;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfUnderline;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfStrikeOut;
            [MarshalAs(UnmanagedType.U1)]
            public FontCharSet lfCharSet;
            [MarshalAs(UnmanagedType.U1)]
            public FontPrecision lfOutPrecision;
            [MarshalAs(UnmanagedType.U1)]
            public FontClipPrecision lfClipPrecision;
            [MarshalAs(UnmanagedType.U1)]
            public FontQuality lfQuality;
            [MarshalAs(UnmanagedType.U1)]
            public FontPitchAndFamily lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("LOGFONT\n");
                sb.AppendFormat("   lfHeight: {0}\n", lfHeight);
                sb.AppendFormat("   lfWidth: {0}\n", lfWidth);
                sb.AppendFormat("   lfEscapement: {0}\n", lfEscapement);
                sb.AppendFormat("   lfOrientation: {0}\n", lfOrientation);
                sb.AppendFormat("   lfWeight: {0}\n", lfWeight);
                sb.AppendFormat("   lfItalic: {0}\n", lfItalic);
                sb.AppendFormat("   lfUnderline: {0}\n", lfUnderline);
                sb.AppendFormat("   lfStrikeOut: {0}\n", lfStrikeOut);
                sb.AppendFormat("   lfCharSet: {0}\n", lfCharSet);
                sb.AppendFormat("   lfOutPrecision: {0}\n", lfOutPrecision);
                sb.AppendFormat("   lfClipPrecision: {0}\n", lfClipPrecision);
                sb.AppendFormat("   lfQuality: {0}\n", lfQuality);
                sb.AppendFormat("   lfPitch: {0}\n", lfPitchAndFamily.GetPitch());
                sb.AppendFormat("   lfFamily: {0}\n", lfPitchAndFamily.GetFamily());
                sb.AppendFormat("   lfFaceName: {0}\n", lfFaceName);

                return sb.ToString();
            }
        }
    }
}
