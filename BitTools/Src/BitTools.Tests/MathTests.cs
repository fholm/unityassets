using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitTools.Tests
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void TestFindLowBitLoop()
        {
            Assert.AreEqual(-1, BitMath.FindLowBitSetLoop(0));

            for (int i = 0; i < 31; ++i)
            {
                uint v = unchecked((uint)1 << i);
                Assert.AreEqual(i, BitMath.FindLowBitSetLoop(v));
            }
        }

        [TestMethod]
        public void TestFindLowBitMod37()
        {
            Assert.AreEqual(32, BitMath.FindLowBitSetMod37(0));

            for (int i = 0; i < 31; ++i)
            {
                uint v = unchecked((uint)1 << i);
                Assert.AreEqual(i, BitMath.FindLowBitSetMod37(v));
            }
        }

        [TestMethod]
        public void TestFindLowBitModDeBruijn()
        {
            // No bit set and first bit set gives same result using DeBruijn
            Assert.AreEqual(0, BitMath.FindLowBitSetDeBruijn(0));
            Assert.AreEqual(0, BitMath.FindLowBitSetDeBruijn(1));

            for (int i = 0; i < 31; ++i)
            {
                uint v = unchecked((uint)1 << i);
                Assert.AreEqual(i, BitMath.FindLowBitSetDeBruijn(v));
            }
        }

        [TestMethod]
        public void TestBitAndByteCalculations()
        {
            Assert.IsFalse(BitMath.IsPowerOf2(0));
            Assert.IsTrue(BitMath.IsPowerOf2(1));
            Assert.IsTrue(BitMath.IsPowerOf2(2));
            Assert.IsFalse(BitMath.IsPowerOf2(3));
            Assert.IsTrue(BitMath.IsPowerOf2(4));
            Assert.IsFalse(BitMath.IsPowerOf2(5));
            Assert.IsFalse(BitMath.IsPowerOf2(6));

            Assert.AreEqual(0, BitMath.RequiredBytes(0));

            for (int i = 1; i <= 8; ++i)
            {
                Assert.AreEqual(1, BitMath.RequiredBytes(i));
            }

            Assert.AreEqual(2, BitMath.RequiredBytes(9));
            Assert.AreEqual(2, BitMath.RequiredBytes(16));
            Assert.AreEqual(3, BitMath.RequiredBytes(17));

            Assert.AreEqual(0, BitMath.BitsInBytes(0));
            Assert.AreEqual(8, BitMath.BitsInBytes(1));
            Assert.AreEqual(16, BitMath.BitsInBytes(2));
            Assert.AreEqual(24, BitMath.BitsInBytes(3));

            Assert.AreEqual(0, BitMath.RoundToUpperByteBoundry(0));
            Assert.AreEqual(8, BitMath.RoundToUpperByteBoundry(1));
            Assert.AreEqual(8, BitMath.RoundToUpperByteBoundry(2));
            Assert.AreEqual(16, BitMath.RoundToUpperByteBoundry(9));
            Assert.AreEqual(16, BitMath.RoundToUpperByteBoundry(16));
            Assert.AreEqual(24, BitMath.RoundToUpperByteBoundry(17));
        }
    }
}
