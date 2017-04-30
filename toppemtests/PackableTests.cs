using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toppem;
using Xunit;
using Xunit.Abstractions;

namespace toppemtests
{
    public class PackableTests
    {
        private readonly ITestOutputHelper output;

        public PackableTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SubclassesPackFields()
        {
            var subclass = new PackableSubclass(0xfe, 0x1234, 0x23456789);
            Assert.Equal(new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 }, subclass.Pack().ToArray());
        }

        [Fact]
        public void SubclassesUnpackFields()
        {
            var subclass = new PackableSubclass(0xfe, 0x1234, 0x23456789);
            Assert.Equal(new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 }, subclass.Pack().ToArray());
        }
    }

    public class PackableSubclass : Packable
    {
        [FieldOrder(2)]
        public short i16;
        [FieldOrder(1)]
        public byte i8;
        [FieldOrder(3)]
        public int i32;

        public PackableSubclass(byte i8, short i16, int i32)
        {
            this.i8 = i8;
            this.i16 = i16;
            this.i32 = i32;
        }
    }
}
