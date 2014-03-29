using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitTools.Tests
{
    [TestClass]
    public class ReadingWritingTests
    {
        [TestMethod]
        public void TestWriteReadByte()
        {
            //  00000000 00000000 00000000 00000000
            //  |      |        |        |        |
            // 31     24       16        8        0

            byte[] b = new byte[4];

            BitWriter.WriteByte(1, 1, b, 0);
            TestUtils.AssertPattern("00000000 00000000 00000000 00000001", b);

            BitWriter.WriteByte(1, 1, b, 31);
            TestUtils.AssertPattern("10000000 00000000 00000000 00000001", b);

            BitWriter.WriteByte(3, 2, b, 7);
            TestUtils.AssertPattern("10000000 00000000 00000001 10000001", b);

            BitWriter.WriteByte(255, 8, b, 9);
            TestUtils.AssertPattern("10000000 00000001 11111111 10000001", b);

            TestUtils.AssertPattern("00000001", BitReader.ReadByte(2, b, 0));
            TestUtils.AssertPattern("00000001", BitReader.ReadByte(7, b, 0));
            TestUtils.AssertPattern("10000001", BitReader.ReadByte(8, b, 0));
            TestUtils.AssertPattern("11000000", BitReader.ReadByte(8, b, 1));
            TestUtils.AssertPattern("11111000", BitReader.ReadByte(8, b, 4));
            TestUtils.AssertPattern("00000000", BitReader.ReadByte(8, b, 20));
            TestUtils.AssertPattern("10000000", BitReader.ReadByte(8, b, 24));
            TestUtils.AssertPattern("00000000", BitReader.ReadByte(8, b, 23));

            BitWriter.WriteByte(3, 2, b, 23);
            TestUtils.AssertPattern("10000001 10000001 11111111 10000001", b);

            BitWriter.WriteByte(1, 1, b, 25);
            TestUtils.AssertPattern("10000011 10000001 11111111 10000001", b);

            TestUtils.AssertPattern("11000000", BitReader.ReadByte(8, b, 17));
            TestUtils.AssertPattern("01000000", BitReader.ReadByte(7, b, 17));
            TestUtils.AssertPattern("00000001", BitReader.ReadByte(1, b, 31));

            BitWriter.WriteClearBit(b, 10);
            TestUtils.AssertPattern("10000011 10000001 11111011 10000001", b);

            BitWriter.WriteClearBit(b, 12);
            TestUtils.AssertPattern("10000011 10000001 11101011 10000001", b);

            BitWriter.WriteSetBit(b, 10);
            TestUtils.AssertPattern("10000011 10000001 11101111 10000001", b);

            for (int i = 0; i < 32; ++i)
                BitWriter.WriteClearBit(b, i);

            TestUtils.AssertPattern("00000000 00000000 00000000 00000000", b);

            BitWriter.WriteSetBit(b, 22);
            BitWriter.WriteSetBit(b, 31);
            TestUtils.AssertPattern("10000000 01000000 00000000 00000000", b);

            BitWriter.WriteByte(7, 3, b, 23);
            TestUtils.AssertPattern("10000011 11000000 00000000 00000000", b);
            TestUtils.AssertPattern("00001111", BitReader.ReadByte(4, b, 22));
        }

        [TestMethod]
        public void TestWriteReadBytes()
        {
            //  00000000 00000000 00000000 00000000 00000000
            //  |      |        |        |        |        |
            // 39     32       24       16        8        0

            byte[] b = new byte[5];
            byte[] s = new byte[3];

            s[0] = 255;
            s[1] = 255;
            s[2] = 255;

            BitWriter.WriteBytes(s, b, 0);
            TestUtils.AssertPattern("00000000 00000000 11111111 11111111 11111111", b);

            BitWriter.WriteBytes(s, 0, 1, b, 25);
            TestUtils.AssertPattern("00000001 11111110 11111111 11111111 11111111", b);

            BitMath.ClearBytes(b);
            TestUtils.AssertPattern("00000000 00000000 00000000 00000000 00000000", b);

            BitWriter.WriteSetBit(b, 0);
            BitWriter.WriteSetBit(b, 19);
            TestUtils.AssertPattern("00000000 00000000 00001000 00000000 00000001", b);

            BitWriter.WriteBytes(s, 0, 2, b, 2);
            TestUtils.AssertPattern("00000000 00000000 00001011 11111111 11111101", b);

            TestUtils.AssertPattern("11111101", BitReader.ReadBytes(b, 1, 0));
            TestUtils.AssertPattern("11111111 11111101", BitReader.ReadBytes(b, 2, 0));
            TestUtils.AssertPattern("11111111 11111110", BitReader.ReadBytes(b, 2, 1));
            TestUtils.AssertPattern("11111111 11111111", BitReader.ReadBytes(b, 2, 2));
            TestUtils.AssertPattern("01111111 11111111", BitReader.ReadBytes(b, 2, 3));
            TestUtils.AssertPattern("00000101 11111111 11111110", BitReader.ReadBytes(b, 3, 1));

            BitMath.ClearBytes(s);
            BitMath.ClearBytes(b);

            TestUtils.AssertPattern("00000000 00000000 00000000", s);
            TestUtils.AssertPattern("00000000 00000000 00000000 00000000 00000000", b);

            s[0] = 1 | 2 | 64;
            s[1] = 2 | 4 | 8 | 128;
            s[2] = 128 | 32 | 16;

            TestUtils.AssertPattern("10110000 10001110 01000011", s);

            BitWriter.WriteBytes(new byte[]{ 255, 255, 255 }, b, 0);
            TestUtils.AssertPattern("00000000 00000000 11111111 11111111 11111111", b);

            BitWriter.WriteBytes(s, b, 0);
            TestUtils.AssertPattern("00000000 00000000 10110000 10001110 01000011", b);

            BitWriter.WriteBytes(s, 1, 1, b, 25);
            TestUtils.AssertPattern("00000001 00011100 10110000 10001110 01000011", b);
        }
    }
}
