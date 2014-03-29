using System;
using System.Text;

namespace BitTools
{
    public static class BitDisplay
    {
        static void ByteToString(byte value, StringBuilder sb)
        {
            ByteToString(value, 8, sb);
        }

        static void ByteToString(byte value, int bits, StringBuilder sb)
        {
#if DEBUG
            if (bits < 1 || bits > 8)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 8");
            }
#endif

            for (int i = (bits - 1); i >= 0; --i)
            {
                if (((1 << i) & value) == 0)
                {
                    sb.Append('0');
                }
                else
                {
                    sb.Append('1');
                }
            }
        }

        public static string ByteToString(byte value, int bits)
        {
            StringBuilder sb = new StringBuilder(8);
            ByteToString(value, bits, sb);
            return sb.ToString();
        }

        public static string ByteToString(byte value)
        {
            return ByteToString(value, 8);
        }

        public static string UShortToString(ushort value)
        {
            StringBuilder sb = new StringBuilder(17);

            ByteToString((byte)(value >> 8), sb);
            sb.Append(' ');
            ByteToString((byte)value, sb);

            return sb.ToString();
        }

        public static string UIntToString(uint value)
        {
            StringBuilder sb = new StringBuilder(35);

            ByteToString((byte)(value >> 24), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 16), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 8), sb);
            sb.Append(' ');
            ByteToString((byte)value, sb);

            return sb.ToString();
        }

        public static string ULongToString(ulong value)
        {
            StringBuilder sb = new StringBuilder(71);

            ByteToString((byte)(value >> 56), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 48), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 40), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 32), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 24), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 16), sb);
            sb.Append(' ');
            ByteToString((byte)(value >> 8), sb);
            sb.Append(' ');
            ByteToString((byte)value, sb);

            return sb.ToString();
        }

        public static string BytesToString(byte[] values)
        {
            StringBuilder sb = new StringBuilder(
                (values.Length * 8) + Math.Max(0, (values.Length - 1))
            );

            for (int i = values.Length-1; i >= 0; --i)
            {
                sb.Append(ByteToString(values[i]));

                if (i != 0)
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }
    }
}
