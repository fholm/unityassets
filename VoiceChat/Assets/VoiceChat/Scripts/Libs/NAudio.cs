/*Microsoft Public License (Ms-PL)

This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

1. Definitions

The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.

A "contribution" is the original software, or any additions or changes to the software.

A "contributor" is any person that distributes its contribution under this license.

"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights

(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.

(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations

(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.

(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.

(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.

(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.

(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
*/

namespace NAudio.Codecs
{
    /// <summary>
    /// A-law encoder
    /// </summary>
    public static class ALawEncoder
    {
        private const int cBias = 0x84;
        private const int cClip = 32635;
        private static readonly byte[] ALawCompressTable = new byte[128]
        {
             1,1,2,2,3,3,3,3,
             4,4,4,4,4,4,4,4,
             5,5,5,5,5,5,5,5,
             5,5,5,5,5,5,5,5,
             6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,
             6,6,6,6,6,6,6,6,
             7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7,
             7,7,7,7,7,7,7,7
        };

        /// <summary>
        /// Encodes a single 16 bit sample to a-law
        /// </summary>
        /// <param name="sample">16 bit PCM sample</param>
        /// <returns>a-law encoded byte</returns>
        public static byte LinearToALawSample(short sample)
        {
            int sign;
            int exponent;
            int mantissa;
            byte compressedByte;

            sign = ((~sample) >> 8) & 0x80;
            if (sign == 0)
                sample = (short)-sample;
            if (sample > cClip)
                sample = cClip;
            if (sample >= 256)
            {
                exponent = (int)ALawCompressTable[(sample >> 8) & 0x7F];
                mantissa = (sample >> (exponent + 3)) & 0x0F;
                compressedByte = (byte)((exponent << 4) | mantissa);
            }
            else
            {
                compressedByte = (byte)(sample >> 4);
            }
            compressedByte ^= (byte)(sign ^ 0x55);
            return compressedByte;
        }
    }

    /// <summary>
    /// a-law decoder
    /// based on code from:
    /// http://hazelware.luggle.com/tutorials/mulawcompression.html
    /// </summary>
    public class ALawDecoder
    {
        /// <summary>
        /// only 512 bytes required, so just use a lookup
        /// </summary>
        private static readonly short[] ALawDecompressTable = new short[256]
        {
             -5504, -5248, -6016, -5760, -4480, -4224, -4992, -4736,
             -7552, -7296, -8064, -7808, -6528, -6272, -7040, -6784,
             -2752, -2624, -3008, -2880, -2240, -2112, -2496, -2368,
             -3776, -3648, -4032, -3904, -3264, -3136, -3520, -3392,
             -22016,-20992,-24064,-23040,-17920,-16896,-19968,-18944,
             -30208,-29184,-32256,-31232,-26112,-25088,-28160,-27136,
             -11008,-10496,-12032,-11520,-8960, -8448, -9984, -9472,
             -15104,-14592,-16128,-15616,-13056,-12544,-14080,-13568,
             -344,  -328,  -376,  -360,  -280,  -264,  -312,  -296,
             -472,  -456,  -504,  -488,  -408,  -392,  -440,  -424,
             -88,   -72,   -120,  -104,  -24,   -8,    -56,   -40,
             -216,  -200,  -248,  -232,  -152,  -136,  -184,  -168,
             -1376, -1312, -1504, -1440, -1120, -1056, -1248, -1184,
             -1888, -1824, -2016, -1952, -1632, -1568, -1760, -1696,
             -688,  -656,  -752,  -720,  -560,  -528,  -624,  -592,
             -944,  -912,  -1008, -976,  -816,  -784,  -880,  -848,
              5504,  5248,  6016,  5760,  4480,  4224,  4992,  4736,
              7552,  7296,  8064,  7808,  6528,  6272,  7040,  6784,
              2752,  2624,  3008,  2880,  2240,  2112,  2496,  2368,
              3776,  3648,  4032,  3904,  3264,  3136,  3520,  3392,
              22016, 20992, 24064, 23040, 17920, 16896, 19968, 18944,
              30208, 29184, 32256, 31232, 26112, 25088, 28160, 27136,
              11008, 10496, 12032, 11520, 8960,  8448,  9984,  9472,
              15104, 14592, 16128, 15616, 13056, 12544, 14080, 13568,
              344,   328,   376,   360,   280,   264,   312,   296,
              472,   456,   504,   488,   408,   392,   440,   424,
              88,    72,   120,   104,    24,     8,    56,    40,
              216,   200,   248,   232,   152,   136,   184,   168,
              1376,  1312,  1504,  1440,  1120,  1056,  1248,  1184,
              1888,  1824,  2016,  1952,  1632,  1568,  1760,  1696,
              688,   656,   752,   720,   560,   528,   624,   592,
              944,   912,  1008,   976,   816,   784,   880,   848
        };

        /// <summary>
        /// Converts an a-law encoded byte to a 16 bit linear sample
        /// </summary>
        /// <param name="aLaw">a-law encoded byte</param>
        /// <returns>Linear sample</returns>
        public static short ALawToLinearSample(byte aLaw)
        {
            return ALawDecompressTable[aLaw];
        }
    }
}
