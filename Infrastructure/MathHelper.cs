using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConsole.Infrastructure
{
    public static class ConversionHelper
    {
        public static uint EnsurePositive(this int @int)
        {
            if (@int < 0)
                throw new ArgumentOutOfRangeException("int", @int, "MUST >= 0");

            return (uint)@int;
        }

        public static byte EnsureByte(this int @int)
        {
            return @int.EnsurePositive().EnsureByte();
        }

        public static byte EnsureByte(this uint @uint)
        {
            return checked((byte)@uint);
        }

        public static ushort EnsureUInt16(this uint @uint)
        {
            return checked((ushort)@uint);
        }

        public static ushort EnsureUInt16(this int @int)
        {
            return @int.EnsurePositive().EnsureUInt16();
        }

        public static int EnsureInt32(this uint @uint)
        {
            return checked((int)@uint);
        }

        public static int EnsureInt32(this long @long)
        {
            return checked((int)@long);
        }
    }
}
