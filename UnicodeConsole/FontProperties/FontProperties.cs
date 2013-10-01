using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole.FontProperties
{
    public enum FontWeight : int
    {
        FW_DONTCARE = 0,
        FW_THIN = 100,
        FW_EXTRALIGHT = 200,
        FW_LIGHT = 300,
        FW_NORMAL = 400,
        FW_MEDIUM = 500,
        FW_SEMIBOLD = 600,
        FW_BOLD = 700,
        FW_EXTRABOLD = 800,
        FW_HEAVY = 900,
    }
    public enum FontCharSet : byte
    {
        ANSI_CHARSET = 0,
        DEFAULT_CHARSET = 1,
        SYMBOL_CHARSET = 2,
        SHIFTJIS_CHARSET = 128,
        HANGEUL_CHARSET = 129,
        HANGUL_CHARSET = 129,
        GB2312_CHARSET = 134,
        CHINESEBIG5_CHARSET = 136,
        OEM_CHARSET = 255,
        JOHAB_CHARSET = 130,
        HEBREW_CHARSET = 177,
        ARABIC_CHARSET = 178,
        GREEK_CHARSET = 161,
        TURKISH_CHARSET = 162,
        VIETNAMESE_CHARSET = 163,
        THAI_CHARSET = 222,
        EASTEUROPE_CHARSET = 238,
        RUSSIAN_CHARSET = 204,
        MAC_CHARSET = 77,
        BALTIC_CHARSET = 186,
    }
    public enum FontPrecision : byte
    {
        OUT_DEFAULT_PRECIS = 0,
        OUT_STRING_PRECIS = 1,
        OUT_CHARACTER_PRECIS = 2,
        OUT_STROKE_PRECIS = 3,
        OUT_TT_PRECIS = 4,
        OUT_DEVICE_PRECIS = 5,
        OUT_RASTER_PRECIS = 6,
        OUT_TT_ONLY_PRECIS = 7,
        OUT_OUTLINE_PRECIS = 8,
        OUT_SCREEN_OUTLINE_PRECIS = 9,
        OUT_PS_ONLY_PRECIS = 10,
    }
    public enum FontClipPrecision : byte
    {
        CLIP_DEFAULT_PRECIS = 0,
        CLIP_CHARACTER_PRECIS = 1,
        CLIP_STROKE_PRECIS = 2,
        CLIP_MASK = 0xf,
        CLIP_LH_ANGLES = (1 << 4),
        CLIP_TT_ALWAYS = (2 << 4),
        CLIP_DFA_DISABLE = (4 << 4),
        CLIP_EMBEDDED = (8 << 4),
    }
    public enum FontQuality : byte
    {
        DEFAULT_QUALITY = 0,
        DRAFT_QUALITY = 1,
        PROOF_QUALITY = 2,
        NONANTIALIASED_QUALITY = 3,
        ANTIALIASED_QUALITY = 4,
        CLEARTYPE_QUALITY = 5,
        CLEARTYPE_NATURAL_QUALITY = 6,
    }
    [Flags]
    public enum FontPitchAndFamily : byte
    {
        DEFAULT_PITCH = 0,
        FIXED_PITCH = 1,
        VARIABLE_PITCH = 2,
        FF_DONTCARE = (0 << 4),
        FF_ROMAN = (1 << 4),
        FF_SWISS = (2 << 4),
        FF_MODERN = (3 << 4),
        FF_SCRIPT = (4 << 4),
        FF_DECORATIVE = (5 << 4),
    }

    public enum FontPitch : byte
    {
        DEFAULT_PITCH = 0,
        FIXED_PITCH = 1,
        VARIABLE_PITCH = 2,
    }

    public enum LogFontFamily : byte
    {
        FF_DONTCARE = 0,
        FF_ROMAN = 1,
        FF_SWISS = 2,
        FF_MODERN = 3,
        FF_SCRIPT = 4,
        FF_DECORATIVE = 5,
    }

    public static class FontPitchAndFamilyEx
    {
        public static FontPitch GetPitch(this FontPitchAndFamily @this)
        {
            return (FontPitch)((byte)@this & 0xf);
        }

        public static LogFontFamily GetFamily(this FontPitchAndFamily @this)
        {
            return (LogFontFamily)(((byte)@this >> 4) & 0x1f);
        }
    }

    public static class FontEx
    {
        public static bool IsMonospaced(this Font @this, Graphics graphics)
        {
            var widthI = graphics.MeasureString("i", @this).Width;
            var widthW = graphics.MeasureString("W", ​@this).Width;
            return widthI == widthW;
        }

    }
}