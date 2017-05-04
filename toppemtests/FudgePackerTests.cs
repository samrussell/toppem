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
    public class FudgePackerTests
    {
        private readonly ITestOutputHelper output;

        public FudgePackerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void FudgePackerPackFields()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 };
            Assert.Equal(serialisedData, new FudgePacker().Pack(packable).ToArray());
        }

        [Fact]
        public void FudgePackerUnpackFields()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] { 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89 };
            Assert.Equal(packable, new FudgePacker().Unpack(typeof(Packable), serialisedData));
        }

        [Fact]
        public void FudgePackerRecursivelyPackFields()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var recursivePackable = new RecursivePackable(0xab, 0x2468, packable, 0x13579bdf);
            var serialisedData = new byte[] {
                0xab,
                0x24, 0x68,
                0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
                0x13, 0x57, 0x9b, 0xdf,
            };
            Assert.Equal(serialisedData, new FudgePacker().Pack(recursivePackable).ToArray());
        }

        [Fact]
        public void FudgePackerRecursivelyUnpackFields()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var recursivePackable = new RecursivePackable(0xab, 0x2468, packable, 0x13579bdf);
            var serialisedData = new byte[] {
                0xab,
                0x24, 0x68,
                0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
                0x13, 0x57, 0x9b, 0xdf,
            };
            Assert.Equal(recursivePackable, new FudgePacker().Unpack(typeof(RecursivePackable), serialisedData));
        }

        [Fact]
        public void FudgePackerPackTlvs()
        {
            var packable = new TlvPackable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] {
                0xad, 0x00, 0x07, 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
            };
            Assert.Equal(serialisedData, new FudgePacker().Pack(packable).ToArray());
        }

        [Fact]
        public void FudgePackerUnpackTlvs()
        {
            var packable = new TlvPackable(0xfe, 0x1234, 0x23456789);
            var serialisedData = new byte[] {
                0xad, 0x00, 0x07, 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
            };
            Assert.Equal(packable, new FudgePacker().Unpack(typeof(TlvPackable), serialisedData));
        }

        [Fact]
        public void FudgePackerRecursivelyPackTlvs()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var recursivePackable = new RecursivePackable(0xab, 0x2468, packable, 0x13579bdf);
            var serialisedData = new byte[] {
                0xab,
                0x24, 0x68,
                0xad, 0x00, 0x07, 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
                0x13, 0x57, 0x9b, 0xdf,
            };
            Assert.Equal(serialisedData, new FudgePacker().Pack(recursivePackable).ToArray());
        }

        [Fact]
        public void FudgePackerRecursivelyUnpackTlvs()
        {
            var packable = new Packable(0xfe, 0x1234, 0x23456789);
            var recursivePackable = new RecursivePackable(0xab, 0x2468, packable, 0x13579bdf);
            var serialisedData = new byte[] {
                0xab,
                0x24, 0x68,
                0xad, 0x00, 0x07, 0xfe, 0x12, 0x34, 0x23, 0x45, 0x67, 0x89,
                0x13, 0x57, 0x9b, 0xdf,
            };
            Assert.Equal(recursivePackable, new FudgePacker().Unpack(typeof(RecursivePackable), serialisedData));
        }
    }

    public class Packable
    {
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(2)]
        public ushort i16;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(1)]
        public byte i8;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(3)]
        public uint i32;
        public byte Type = 0xaf;

        public Packable(byte i8, ushort i16, uint i32)
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
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(2)]
        public ushort i16;
        [FieldOrder(3)]
        [Packable]
        public Packable packable;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(1)]
        public byte i8;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(4)]
        public uint i32;

        public RecursivePackable(byte i8, ushort i16, Packable packable, uint i32)
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
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(1)]
        public byte i8;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(2)]
        public ushort i16;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(3)]
        public uint i32;

        public TlvPackable(byte i8, ushort i16, uint i32)
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

    public class RecursiveTlvPackable
    {
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(2)]
        public ushort i16;
        [FieldOrder(3)]
        [Packable]
        public TlvPackable tlvPackable;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(1)]
        public byte i8;
        [PacksWith(typeof(CavemanPacker))]
        [FieldOrder(4)]
        public uint i32;

        public RecursiveTlvPackable(byte i8, ushort i16, TlvPackable tlvPackable, uint i32)
        {
            this.i8 = i8;
            this.i16 = i16;
            this.i32 = i32;
            this.tlvPackable = tlvPackable;
        }

        public override bool Equals(object obj)
        {
            var other = (RecursiveTlvPackable)obj;
            return (obj != null)
                && (i8 == other.i8)
                && (i16 == other.i16)
                && (i32 == other.i32)
                && (tlvPackable.Equals(other.tlvPackable));
        }

        public override int GetHashCode()
        {
            return i8.GetHashCode() ^ i16.GetHashCode() ^ i32.GetHashCode() ^ tlvPackable.GetHashCode();
        }
    }
}
