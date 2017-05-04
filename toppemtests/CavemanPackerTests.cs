using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toppem;
using Xunit;
using Xunit.Abstractions;
using System.Reflection;
using System.IO;

namespace toppemtests
{
    public class CavemanPackerTests
    {
        private readonly ITestOutputHelper output;

        public CavemanPackerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CavemanPackerPacksByte()
        {
            byte packable = 0xab;
            var serialisedData = new byte[] { 0xab };
            Assert.Equal(serialisedData, CavemanPacker.Pack(packable).ToArray());
        }

        [Fact]
        public void CavemanPackerPacksShort()
        {
            ushort packable = 0xabcd;
            var serialisedData = new byte[] { 0xab, 0xcd };
            Assert.Equal(serialisedData, CavemanPacker.Pack(packable).ToArray());
        }

        [Fact]
        public void CavemanPackerPacksInt()
        {
            uint packable = 0xabcd1234;
            var serialisedData = new byte[] { 0xab, 0xcd, 0x12, 0x34 };
            Assert.Equal(serialisedData, CavemanPacker.Pack(packable).ToArray());
        }

        [Fact]
        public void CavemanPackerUnpacksByte()
        {
            byte packable = 0xab;
            var serialisedData = new byte[] { 0xab };
            Assert.Equal(packable, CavemanPacker.Unpack<byte>(new MemoryStream(serialisedData)));
        }

        [Fact]
        public void CavemanPackerUnpacksShort()
        {
            ushort packable = 0xabcd;
            var serialisedData = new byte[] { 0xab, 0xcd };
            Assert.Equal(packable, CavemanPacker.Unpack<ushort>(new MemoryStream(serialisedData)));
        }

        [Fact]
        public void CavemanPackerUnpacksInt()
        {
            uint packable = 0xabcd1234;
            var serialisedData = new byte[] { 0xab, 0xcd, 0x12, 0x34 };
            Assert.Equal(packable, CavemanPacker.Unpack<uint>(new MemoryStream(serialisedData)));
        }
    }
}
