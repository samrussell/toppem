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
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 };
            Assert.Equal(serialisedData, new BgpPacker().Pack(packable).ToArray());
        }

        [Fact]
        public void SubclassesUnpackFields()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 };
            Assert.Equal(packable, new BgpPacker().Unpack(typeof(Packable), serialisedData));
        }

        [Fact]
        public void SubclassesRecursivelyPackFields()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var recursivePackable = new RecursivePackable(0xab, 0x2468, packable, 0x13579bdf);
            var serialisedData = new byte[] {
                0xab,
                0x24, 0x68,
                0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
                0x13, 0x57, 0x9b, 0xdf,
            };
            Assert.Equal(serialisedData, new BgpPacker().Pack(recursivePackable).ToArray());
        }

        [Fact]
        public void SubclassesRecursivelyUnpackFields()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var recursivePackable = new RecursivePackable(0xab, 0x2468, packable, 0x13579bdf);
            var serialisedData = new byte[] {
                0xab,
                0x24, 0x68,
                0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
                0x13, 0x57, 0x9b, 0xdf,
            };
            Assert.Equal(recursivePackable, new BgpPacker().Unpack(typeof(RecursivePackable), serialisedData));
        }

        [Fact]
        public void SubclassesPackTlvs()
        {
            var packable = new TlvPackable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] {
                0xad, 0x00, 0x07, 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
            };
            Assert.Equal(serialisedData, new BgpPacker().Pack(packable).ToArray());
        }

        [Fact]
        public void SubclassesUnpackTlvs()
        {
            var packable = new TlvPackable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] {
                0xad, 0x00, 0x07, 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
            };
            Assert.Equal(packable, new BgpPacker().Unpack(typeof(TlvPackable), serialisedData));
        }

        /*[Fact]
        public void SubclassesRecursivelyUnpackFieldsAsTlvs()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var recursivePackable = new RecursivePackable(0xab, 0x2468, packable, 0x13579bdf);
            var serialisedData = new byte[] {
                0xab,
                0x24, 0x68,
                0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
                0x13, 0x57, 0x9b, 0xdf,
            };
            Assert.Equal(recursivePackable, new BgpPacker().Unpack(typeof(RecursivePackable), serialisedData));
        }*/
    }

    public class Packable
    {
        [FieldOrder(2)]
        public short i16;
        [FieldOrder(1)]
        public byte i8;
        [FieldOrder(3)]
        public int i32;
        public byte Type = 0xaf;

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

    public class RecursivePackable
    {
        [FieldOrder(2)]
        public short i16;
        [FieldOrder(3)]
        [Packable]
        public Packable packable;
        [FieldOrder(1)]
        public byte i8;
        [FieldOrder(4)]
        public int i32;

        public RecursivePackable(byte i8, short i16, Packable packable, int i32)
        {
            this.i8 = i8;
            this.i16 = i16;
            this.i32 = i32;
            this.packable = packable;
        }

        public override bool Equals(object obj)
        {
            var other = (RecursivePackable)obj;
            return (obj != null)
                && (i8 == other.i8)
                && (i16 == other.i16)
                && (i32 == other.i32)
                && (packable.Equals(other.packable));
        }

        public override int GetHashCode()
        {
            return i8.GetHashCode() ^ i16.GetHashCode() ^ i32.GetHashCode() ^ packable.GetHashCode();
        }
    }

    [Tlv(typeof(byte), typeof(short), 0xad)]
    public class TlvPackable
    {
        [FieldOrder(1)]
        public byte i8;
        [FieldOrder(2)]
        public short i16;
        [FieldOrder(3)]
        public int i32;

        public TlvPackable(byte i8, short i16, int i32)
        {
            this.i8 = i8;
            this.i16 = i16;
            this.i32 = i32;
        }

        public override bool Equals(object obj)
        {
            var other = (TlvPackable)obj;
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
