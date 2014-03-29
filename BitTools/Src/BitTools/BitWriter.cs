using System;

namespace BitTools
{
    public static class BitWriter
    {
        public static void WriteByte(byte value, int bits, byte[] to, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 8)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 8");
            }
#endif
            // Mask out all the bits we dont want
            value = (byte)(value & (0xFF >> (8 - bits)));

            int p = pos >> 3;
            int bitsUsed = pos & 0x7; // mod 8
            int bitsFree = 8 - bitsUsed;
            int bitsLeft = bitsFree - bits;

            // Fast path, everything fits in the first byte
            if (bitsLeft >= 0)
            {
                int mask = (0xFF >> bitsFree) | (0xFF << (8 - bitsLeft));

                to[p] = (byte)(
                    // Mask out lower and upper bits
                    (to[p] & mask) |

                    // Insert new bits
                    (value << bitsUsed)
                );

                return;
            }

            to[p] = (byte)(
                // Mask out upper bits
                (to[p] & (0xFF >> bitsFree)) |

                // Write the lower bits to the upper bits in the first byte
                (value << bitsUsed)
            );

            p += 1;

            to[p] = (byte)(
                // Mask out lower bits
                (to[p] & (0xFF << (bits - bitsFree))) |

                // Write the upper bits to the lower bits of the second byte
                (value >> bitsFree)
            );
        }

        public static void WriteByte(byte value, byte[] to, int pos)
        {
            WriteByte(value, 8, to, pos);
        }

        public static void WriteByte(byte value, int bits, byte[] to, ref int pos)
        {
            WriteByte(value, bits, to, pos);
            pos += bits;
        }

        public static void WriteByte(byte value, byte[] to, ref int pos)
        {
            WriteByte(value, 8, to, pos);
            pos += 8;
        }

        public static void WriteSetBit(byte[] to, int pos)
        {
            WriteByte(1, 1, to, pos);
        }

        public static void WriteSetBit(byte[] to, ref int pos)
        {
            WriteByte(1, 1, to, pos);
            pos += 1;
        }

        public static void WriteClearBit(byte[] to, int pos)
        {
            WriteByte(0, 1, to, pos);
        }

        public static void WriteClearBit(byte[] to, ref int pos)
        {
            WriteByte(0, 1, to, pos);
            pos += 1;
        }

        public static void WriteUShort(ushort value, int bits, byte[] to, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 16)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 16");
            }
#endif

#if BIG_ENDIAN
            value = (ushort)(((value & 0x0000FF00) >> 8) | ((value & 0x000000FF) << 8));
#endif

            if (bits <= 8)
            {
                WriteByte((byte)value, bits, to, pos);
                return;
            }

            WriteByte((byte)value, 8, to, pos);
            WriteByte((byte)(value >> 8), bits - 8, to, pos + 8);
        }

        public static void WriteUShort(ushort value, byte[] to, int pos)
        {
            WriteUShort(value, 16, to, pos);
        }

        public static void WriteUShort(ushort value, int bits, byte[] to, ref int pos)
        {
            WriteUShort(value, bits, to, pos);
            pos += bits;
        }

        public static void WriteUShort(ushort value, byte[] to, ref int pos)
        {
            WriteUShort(value, 16, to, pos);
            pos += 16;
        }

        public static void WriteUInt(uint value, int bits, byte[] to, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 32)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 32");
            }
#endif

#if BIG_ENDIAN
            value = 
                ((value & 0xFF000000) >> 24) | 
                ((value & 0x00FF0000) >> 8)  |
                ((value & 0x0000FF00) << 8)  | 
                ((value & 0x000000FF) << 24);
#endif

            int shift = 0;

            while (true)
            {
                if (bits <= 8)
                {
                    WriteByte((byte)(value >> shift), bits, to, pos + shift);
                    return;
                }

                WriteByte((byte)(value >> shift), 8, to, pos + shift);
                shift += 8;
                bits -= 8;
            }
        }

        public static void WriteUInt(uint value, byte[] to, int pos)
        {
            WriteUInt(value, 32, to, pos);
        }

        public static void WriteUInt(uint value, int bits, byte[] to, ref int pos)
        {
            WriteUInt(value, bits, to, pos);
            pos += bits;
        }

        public static void WriteUInt(uint value, byte[] to, ref int pos)
        {
            WriteUInt(value, 32, to, pos);
            pos += 32;
        }

        public static void WriteULong(ulong value, int bits, byte[] to, int pos)
        {
#if DEBUG
            if (bits < 1 || bits > 64)
            {
                throw new ArgumentOutOfRangeException("bits", "Must be between 1 and 64");
            }
#endif

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
#endif

            int shift = 0;

            while (true)
            {
                if (bits <= 8)
                {
                    WriteByte((byte)(value >> shift), bits, to, pos + shift);
                    return;
                }

                WriteByte((byte)(value >> shift), 8, to, pos + shift);
                shift += 8;
                bits -= 8;
            }
        }

        public static void WriteULong(ulong value, byte[] to, int pos)
        {
            WriteULong(value, 64, to, pos);
        }

        public static void WriteULong(ulong value, int bits, byte[] to, ref int pos)
        {
            WriteULong(value, bits, to, pos);
            pos += bits;
        }

        public static void WriteULong(ulong value, byte[] to, ref int pos)
        {
            WriteULong(value, 64, to, pos);
            pos += 64;
        }

        public static void WriteBytes(byte[] from, int offset, int count, byte[] to, int pos)
        {
#if DEBUG
            if (offset < 0 || offset >= from.Length)
            {
                throw new ArgumentOutOfRangeException("offset", "Must be within range of from array");
            }

            if (count < 1 || count > (from.Length - offset))
            {
                throw new ArgumentOutOfRangeException("count", "Must be within range of from array");
            }
#endif

            int p = pos >> 3;
            int bitsUsed = pos % 8;
            int bitsFree = 8 - bitsUsed;

            // Fast path!
            if (bitsUsed == 0)
            {
                Buffer.BlockCopy(from, offset, to, p, count);
                return;
            }

            for (int i = 0; i < count; ++i)
            {
                byte value = from[offset + i];

                to[p] &= (byte)(0xFF >> bitsFree);
                to[p] |= (byte)(value << bitsUsed);

                p += 1;

                to[p] &= (byte)(0xFF << bitsUsed);
                to[p] |= (byte)(value >> bitsFree);
            }
        }

        public static void WriteBytes(byte[] from, byte[] to, int pos)
        {
            WriteBytes(from, 0, from.Length, to, pos);
        }

        public static void WriteBytes(byte[] from, int offset, int count, byte[] to, ref int pos)
        {
            WriteBytes(from, offset, count, to, pos);
            pos += (count * 8);
        }

        public static void WriteBytes(byte[] from, byte[] to, ref int pos)
        {
            WriteBytes(from, 0, from.Length, to, pos);
            pos += (from.Length * 8);
        }
    }
}
