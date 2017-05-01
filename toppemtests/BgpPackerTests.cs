using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toppem;
using Xunit;
using Xunit.Abstractions;
using System.Reflection;

namespace toppemtests
{
    public class BgpPackerTests
    {
        private readonly ITestOutputHelper output;

        public BgpPackerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SubclassesPackFields()
        {
            var subclass = new Packable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 };
            Assert.Equal(serialisedData, new BgpPacker<Packable>().Pack(subclass).ToArray());
        }

        [Fact]
        public void SubclassesUnpackFields()
        {
            var subclass = new Packable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 };
            Assert.Equal(subclass, new BgpPacker<Packable>().Unpack(serialisedData));
        }
    }

    public class Packable
    {
        [FieldOrder(2)]
        public short i16;
        [FieldOrder(1)]
        public byte i8;
        [FieldOrder(3)]
        public int i32;

        public Packable(byte i8, short i16, int i32)
        {
            this.i8 = i8;
            this.i16 = i16;
            this.i32 = i32;
        }

        public override bool Equals(object obj)
        {
            var other = (Packable) obj;
            return (obj != null)
                && (i8 == other.i8)
                && (i16 == other.i16)
                && (i32 == other.i32);
        }

        public override int GetHashCode()
        {
            return i8.GetHashCode() ^ i16.GetHashCode() ^ i32.GetHashCode();
        }
    }
}
