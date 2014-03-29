#define FAST_CONVERT

using System;
using System.Runtime.InteropServices;

namespace BitTools
{
    public static class BitConvert
    {
#if FAST_CONVERT
        [StructLayout(LayoutKind.Explicit)]
        struct Float2Bytes
        {
            [FieldOffset(0)]
            public float Float;
#if BIG_ENDIAN
            [FieldOffset(3)]
            public byte Byte0;

            [FieldOffset(2)]
            public byte Byte1;

            [FieldOffset(1)]
            public byte Byte2;

            [FieldOffset(0)]
            public byte Byte3;
#else

            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(2)]
            public byte Byte2;

            [FieldOffset(3)]
            public byte Byte3;
#endif
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Double2Bytes
        {
            [FieldOffset(0)]
            public double Double;

#if BIG_ENDIAN
            [FieldOffset(7)]
            public byte Byte0;

            [FieldOffset(6)]
            public byte Byte1;

            [FieldOffset(5)]
            public byte Byte2;

            [FieldOffset(4)]
            public byte Byte3;

            [FieldOffset(3)]
            public byte Byte4;

            [FieldOffset(2)]
            public byte Byte5;

            [FieldOffset(1)]
            public byte Byte6;

            [FieldOffset(0)]
            public byte Byte7;
#else
            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(2)]
            public byte Byte2;

            [FieldOffset(3)]
            public byte Byte3;

            [FieldOffset(4)]
            public byte Byte4;

            [FieldOffset(5)]
            public byte Byte5;

            [FieldOffset(6)]
            public byte Byte6;

            [FieldOffset(7)]
            public byte Byte7;
#endif
        }
#endif

        public static void FloatToBytes(float value, out byte b0, out byte b1, out byte b2, out byte b3)
        {
#if FAST_CONVERT
            Float2Bytes f2b = default(Float2Bytes);
            f2b.Float = value;
            b0 = f2b.Byte0;
            b1 = f2b.Byte1;
            b2 = f2b.Byte2;
            b3 = f2b.Byte3;
#else
            byte[] bytes = BitConverter.GetBytes(value);
            
#if BIG_ENDIAN
            b0 = bytes[3];
            b1 = bytes[2];
            b2 = bytes[1];
            b3 = bytes[0];
#else
            b0 = bytes[0];
            b1 = bytes[1];
            b2 = bytes[2];
            b3 = bytes[3];
#endif
#endif
        }

        public static void DoubleToBytes(double value, out byte b0, out byte b1, out byte b2, out byte b3, out byte b4, out byte b5, out byte b6, out byte b7)
        {
#if FAST_CONVERT
            Double2Bytes d2b = default(Double2Bytes);
            d2b.Double = value;
            b0 = d2b.Byte0;
            b1 = d2b.Byte1;
            b2 = d2b.Byte2;
            b3 = d2b.Byte3;
            b4 = d2b.Byte4;
            b5 = d2b.Byte5;
            b6 = d2b.Byte6;
            b7 = d2b.Byte7;
#else
            byte[] bytes = BitConverter.GetBytes(value);
            
#if BIG_ENDIAN
            b0 = bytes[7];
            b1 = bytes[6];
            b2 = bytes[5];
            b3 = bytes[4];
            b4 = bytes[3];
            b5 = bytes[2];
            b6 = bytes[1];
            b7 = bytes[0];
#else
            b0 = bytes[0];
            b1 = bytes[1];
            b2 = bytes[2];
            b3 = bytes[3];
            b4 = bytes[4];
            b5 = bytes[5];
            b6 = bytes[6];
            b7 = bytes[7];
#endif
#endif
        }

        public static float FloatFromBytes(byte b0, byte b1, byte b2, byte b3)
        {
#if FAST_CONVERT
            Float2Bytes f2b = default(Float2Bytes);
            f2b.Byte0 = b0;
            f2b.Byte1 = b1;
            f2b.Byte2 = b2;
            f2b.Byte3 = b3;
            return f2b.Float;
#else
#if BIG_ENDIAN
            return BitConverter.ToSingle(new byte[] { b0, b1, b2, b3 }, 0);
#else
            return BitConverter.ToSingle(new byte[] { b3, b2, b1, b0 }, 0);
#endif
#endif
        }

        public static double DoubleFromBytes(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7)
        {
#if FAST_CONVERT
            Double2Bytes d2b = default(Double2Bytes);
            d2b.Byte0 = b0;
            d2b.Byte1 = b1;
            d2b.Byte2 = b2;
            d2b.Byte3 = b3;
            d2b.Byte4 = b4;
            d2b.Byte5 = b5;
            d2b.Byte6 = b6;
            d2b.Byte7 = b7;
            return d2b.Double;
#else
#if BIG_ENDIAN
            return BitConverter.ToDouble(new byte[] { b7, b6, b5, b4, b3, b2, b1, b0 }, 0);
#else
            return BitConverter.ToDouble(new byte[] { b0, b1, b2, b3, b4, b5, b6, b7 }, 0);
#endif
#endif
        }
    }
}
