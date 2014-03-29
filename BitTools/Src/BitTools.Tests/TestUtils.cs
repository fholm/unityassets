using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitTools.Tests
{
    static class TestUtils
    {
        public static void AssertPattern(string pattern, byte value)
        {
            Assert.AreEqual(pattern, BitDisplay.ByteToString(value));
        }

        public static void AssertPattern(string pattern, byte[] value)
        {
            Assert.AreEqual(pattern, BitDisplay.BytesToString(value));
        }

        public static void AssertPattern(string pattern, ushort value)
        {
            Assert.AreEqual(pattern, BitDisplay.UShortToString(value));
        }

        public static void AssertPattern(string pattern, uint value)
        {
            Assert.AreEqual(pattern, BitDisplay.UIntToString(value));
        }

        public static void AssertPattern(string pattern, ulong value)
        {
            Assert.AreEqual(pattern, BitDisplay.ULongToString(value));
        }
    }
}
