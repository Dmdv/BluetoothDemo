using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class RsaTests
    {
        [TestMethod]
        public void TestEncryption()
        {
            var bytes = BitConverter.GetBytes(1);
            var bytes1 = BitConverter.GetBytes(Int32.MaxValue - 1);
        }
    }
}
