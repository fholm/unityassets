#define UNIT_TEST_API

namespace BitTools
{
    public static class BitMath
    {
#if LOWBIT_MOD37 || UNIT_TEST_API
        static readonly int[] Mod37BitPosition = new int[] {
                32, 0, 1, 26, 2, 23, 27, 0, 3, 16, 24, 30, 28, 11, 0, 13, 4,
                7, 17, 0, 25, 22, 31, 15, 29, 10, 12, 6, 0, 21, 14, 9, 5,
                20, 8, 19, 18
            };
#endif

#if LOWBIT_DEBRUIJN || UNIT_TEST_API
        static readonly int[] MultiplyDeBruijnBitPosition = new int[] {
                0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 
                31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
            };
#endif
        public static void ClearBytes(byte[] bytes)
        {
            if (bytes == null)
                return;

            for (int i = 0; i < bytes.Length; ++i)
                bytes[i] = 0;
        }

        public static bool StructuralEquality(byte[] a, byte[] b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null || a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        public static bool IsPowerOf2(int x)
        {
            return (x != 0) && (x & (x - 1)) == 0;
        }

        public static int RequiredBytes(int bits)
        {
            return (bits + 7) >> 3;
        }

        public static int BitsInBytes(int bytes)
        {
            return bytes << 3;
        }

        public static int RoundToUpperByteBoundry(int bits)
        {
            return ((bits + 7) >> 3) << 3;
        }

#if UNIT_TEST_API
        public static int FindLowBitSetDeBruijn(uint v)
        {
            return MultiplyDeBruijnBitPosition[((uint)((v & -v) * 0x077CB531u)) >> 27];
        }

        public static int FindLowBitSetMod37(uint v)
        {
            return Mod37BitPosition[(-v & v) % 37];
        }
#endif

#if LOWBIT_DEBRUIJN
        public static int FindLowBitSet(uint v)
        {
            return MultiplyDeBruijnBitPosition[((uint)((v & -v) * 0x077CB531u)) >> 27];
        }
#elif LOWBIT_MOD37
        public static int FindLowBitSet(uint v)
        {
            return Mod37BitPosition[(-v & v) % 37];
        }
#else
#if UNIT_TEST_API
        public static int FindLowBitSetLoop(uint v)
#else
        public static int FindLowBitSet(uint v)
#endif
        {
            for (int b = 0; b < 32; ++b)
            {
                if ((v & (1 << b)) != 0)
                {
                    return b;
                }
            }

            return -1;
        }
#endif
    }
}
