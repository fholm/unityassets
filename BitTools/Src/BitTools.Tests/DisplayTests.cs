using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitTools.Tests
{
    [TestClass]
    public class DisplayTests
    {
        [TestMethod]
        public void TestDisplayByte()
        {
            Assert.AreEqual("11111111", BitDisplay.ByteToString(255));
            Assert.AreEqual("00000001", BitDisplay.ByteToString(1));
            Assert.AreEqual("00000010", BitDisplay.ByteToString(1 << 1));
            Assert.AreEqual("10000000", BitDisplay.ByteToString(128));
            Assert.AreEqual("01000000", BitDisplay.ByteToString(128 >> 1));
            Assert.AreEqual("10000001", BitDisplay.ByteToString(128 | 1));
        }

        [TestMethod]
        public void TestDisplayUShort()
        {
            Assert.AreEqual("00000000 11111111", BitDisplay.UShortToString(255));
            Assert.AreEqual("00000000 00000001", BitDisplay.UShortToString(1));
            Assert.AreEqual("00000000 00000010", BitDisplay.UShortToString(1 << 1));
            Assert.AreEqual("10000000 00000000", BitDisplay.UShortToString(1 << 15));
            Assert.AreEqual("10000000 00000001", BitDisplay.UShortToString(1 | 1 << 15));
        }

        [TestMethod]
        public void TestDisplayUInt()
        {
            uint topbit = unchecked((uint)(1 << 31));

            Assert.AreEqual("00000000 00000000 00000000 11111111", BitDisplay.UIntToString(255));
            Assert.AreEqual("00000000 00000000 00000000 00000001", BitDisplay.UIntToString(1));
            Assert.AreEqual("00000000 00000000 00000000 00000010", BitDisplay.UIntToString(1 << 1));
            Assert.AreEqual("00000000 00000000 00000000 10000000", BitDisplay.UIntToString(1 << 7));
            Assert.AreEqual("00000000 00000000 10000000 00000000", BitDisplay.UIntToString(1 << 15));
            Assert.AreEqual("00000000 10000000 00000000 00000000", BitDisplay.UIntToString(1 << 23));
            Assert.AreEqual("10000000 00000000 00000000 00000000", BitDisplay.UIntToString(topbit));
            Assert.AreEqual("10000000 00000000 00000000 00000001", BitDisplay.UIntToString(1 | topbit));
        }

        [TestMethod]
        public void TestDisplayULong()
        {
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 11111111", BitDisplay.ULongToString(255UL));
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001", BitDisplay.ULongToString(1UL));
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000010", BitDisplay.ULongToString(1UL << 1));
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 10000000", BitDisplay.ULongToString(1UL << 7));
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 10000000 00000000", BitDisplay.ULongToString(1UL << 15));
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 10000000 00000000 00000000", BitDisplay.ULongToString(1UL << 23));
            Assert.AreEqual("00000000 00000000 00000000 00000000 10000000 00000000 00000000 00000000", BitDisplay.ULongToString(1UL << 31));
            Assert.AreEqual("00000000 00000000 00000000 10000000 00000000 00000000 00000000 00000000", BitDisplay.ULongToString(1UL << 39));
            Assert.AreEqual("00000000 00000000 10000000 00000000 00000000 00000000 00000000 00000000", BitDisplay.ULongToString(1UL << 47));
            Assert.AreEqual("00000000 10000000 00000000 00000000 00000000 00000000 00000000 00000000", BitDisplay.ULongToString(1UL << 55));
            Assert.AreEqual("10000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000", BitDisplay.ULongToString(1UL << 63));
            Assert.AreEqual("10000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001", BitDisplay.ULongToString(1UL | (1UL << 63)));
        }

        [TestMethod]
        public void TestDisplayBytes()
        {
            Assert.AreEqual("00000000 11111111", BitDisplay.BytesToString(new byte[] { 255, 0}));
            Assert.AreEqual("00000001 11111111", BitDisplay.BytesToString(new byte[] { 255, 1}));
            Assert.AreEqual("10000001 11111111", BitDisplay.BytesToString(new byte[] { 255, 128 | 1 }));
            Assert.AreEqual("10000001 11111111 00000001", BitDisplay.BytesToString(new byte[] { 1, 255, 128 | 1 }));
            Assert.AreEqual("10000001 11111111 00000010", BitDisplay.BytesToString(new byte[] { 1 << 1, 255, 128 | 1 }));
            Assert.AreEqual("01000000 11111111 00000010", BitDisplay.BytesToString(new byte[] { 1 << 1, 255, (128 | 1) >> 1 }));
        }
    }
}
