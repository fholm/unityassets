using System;

namespace BitTools
{
    public static class BitReader
    {
        public static byte ReadByte(int bits, byte[] from, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 8)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 8");
            }
#endif

            int p = pos >> 3;
            int bitsUsed = pos % 8;

            if (bitsUsed == 0 && bits == 8)
            {
                return from[p];
            }

            int first = from[p] >> bitsUsed;
            int remainingBits = bits - (8 - bitsUsed);

            if (remainingBits < 1)
            {
                return (byte)(first & (0xFF >> (8 - bits)));
            }

            int second = from[p + 1] & (0xFF >> (8 - remainingBits));
            return (byte)(first | (second << (bits - remainingBits)));
        }

        public static byte ReadByte(byte[] from, int pos)
        {
            return ReadByte(8, from, pos);
        }

        public static byte ReadByte(int bits, byte[] from, ref int pos)
        {
            byte value = ReadByte(bits, from, pos);
            pos += bits;
            return value;
        }

        public static byte ReadByte(byte[] from, ref int pos)
        {
            byte value = ReadByte(8, from, pos);
            pos += 8;
            return value;
        }

        public static byte ReadBit(byte[] from, int pos)
        {
            return ReadByte(1, from, pos);
        }

        public static byte ReadBit(byte[] from, ref int pos)
        {
            byte value = ReadByte(1, from, pos);
            pos += 1;
            return value;
        }

        public static ushort ReadUShort(int bits, byte[] from, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 16)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 16");
            }
#endif

            if (bits <= 8)
            {
                return ReadByte(bits, from, pos);
            }
            else
            {
                byte first = ReadByte(8, from, pos);
                byte second = ReadByte(bits - 8, from, pos + 8);

#if BIG_ENDIAN
                return (ushort)((first << 8) | second);
#else
                return (ushort)(first | (second << 8));
#endif
            }
        }

        public static ushort ReadUShort(byte[] from, int pos)
        {
            return ReadUShort(16, from, pos);
        }

        public static ushort ReadUShort(int bits, byte[] from, ref int pos)
        {
            ushort value = ReadUShort(bits, from, pos);
            pos += bits;
            return value;
        }

        public static ushort ReadUShort(byte[] from, ref int pos)
        {
            ushort value = ReadUShort(16, from, pos);
            pos += 16;
            return value;
        }

        public static uint ReadUInt(int bits, byte[] from, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 32)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 32");
            }
#endif

            int shift = 0;
            uint value = 0;

            while (true)
            {
                if (bits <= 8)
                {
                    value |= (((uint)ReadByte(bits, from, pos + shift)) << shift);
                    break;
                }

                value |= (((uint)ReadByte(8, from, pos + shift)) << shift);
                bits -= 8;
                shift += 8;
            }

#if BIG_ENDIAN
            return
                ((value & 0xFF000000) >> 24) |
                ((value & 0x00FF0000) >> 8)  |
                ((value & 0x0000FF00) << 8)  |
                ((value & 0x000000FF) << 24);
#else
            return value;
#endif
        }

        public static uint ReadUInt(byte[] from, int pos)
        {
            return ReadUInt(32, from, pos);
        }

        public static uint ReadUInt(int bits, byte[] from, ref int pos)
        {
            uint value = ReadUInt(bits, from, pos);
            pos += bits;
            return value;
        }

        public static uint ReadUInt(byte[] from, ref int pos)
        {
            uint value = ReadUInt(32, from, pos);
            pos += 32;
            return value;
        }

        public static ulong ReadULong(int bits, byte[] from, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 64)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 64");
            }
#endif

            int shift = 0;
            ulong value = 0;

            while (true)
            {
                if (bits <= 8)
                {
                    value |= (((uint)ReadByte(bits, from, pos + shift)) << shift);
                    break;
                }

                value |= (((uint)ReadByte(8, from, pos + shift)) << shift);
                bits -= 8;
                shift += 8;
            }

#if BIG_ENDIAN
            value =
                ((value & 0xFF00000000000000L) >> 56) |
                ((value & 0x00FF000000000000L) >> 40) |
                ((value & 0x0000FF0000000000L) >> 24) |
                ((value & 0x000000FF00000000L) >> 8)  |
                ((value & 0x00000000FF000000L) << 8)  |
                ((value & 0x0000000000FF0000L) << 24) |
                ((value & 0x000000000000FF00L) << 40) |
                ((value & 0x00000000000000FFL) << 56);
#else
            return value;
#endif
        }

        public static ulong ReadULong(byte[] from, int pos)
        {
            return ReadULong(64, from, pos);
        }

        public static ulong ReadULong(int bits, byte[] from, ref int pos)
        {
            ulong value = ReadULong(bits, from, pos);
            pos += bits;
            return value;
        }

        public static ulong ReadULong(byte[] from, ref int pos)
        {
            ulong value = ReadULong(64, from, pos);
            pos += 64;
            return value;
        }

        public static void ReadBytes(byte[] from, int pos, byte[] to, int offset, int count)
        {
            int p = pos >> 3;
            int bitsUsed = pos % 8;

            if (bitsUsed == 0)
            {
                Buffer.BlockCopy(from, p, to, offset, count);
            }

            int bitsNotUsed = 8 - bitsUsed;

            for (int i = 0; i < count; ++i)
            {
                int first = from[p] >> bitsUsed;

                p += 1;

                int second = from[p] & (255 >> bitsNotUsed);
                to[offset + i] = (byte)(first | (second << bitsNotUsed));
            }
        }

        public static void ReadBytes(byte[] from, int pos, byte[] to)
        {
            ReadBytes(from, pos, to, 0, to.Length);
        }

        public static void ReadBytes(byte[] from, ref int pos, byte[] to, int offset, int count)
        {
            ReadBytes(from, pos, to, offset, count);
            pos += (count * 8);
        }

        public static void ReadBytes(byte[] from, ref int pos, byte[] to)
        {
            ReadBytes(from, pos, to, 0, to.Length);
            pos += (to.Length * 8);
        }

        public static byte[] ReadBytes(byte[] from, int count, int pos)
        {
            byte[] to = new byte[count];
            ReadBytes(from, pos, to, 0, count);
            return to;
        }

        public static byte[] ReadBytes(byte[] from, int count, ref int pos)
        {
            byte[] to = ReadBytes(from, count, pos);
            pos += (count * 8);
            return to;
        }
    }
}