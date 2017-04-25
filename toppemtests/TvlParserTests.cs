using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toppem;
using Xunit;

namespace toppemtests
{
    public class TvlParserTests
    {
        [Fact]
        public void TlvReaderDecodesVarious()
        {
            TlvReaderDecodes1and1();
            TlvReaderDecodes2and1();
            TlvReaderDecodes1and2();
            TlvReaderDecodes2and2();
        }

        void TlvReaderDecodes1and1()
        {
            var serializedTlv = new byte[] { 0xab, 0x05, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            var expectedTlv = new Tlv(0xab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            TlvReaderDecodes(1, 1, serializedTlv, expectedTlv);
        }
        
        void TlvReaderDecodes1and2()
        {
            var serializedTlv = new byte[] { 0xab, 0x05, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            var expectedTlv = new Tlv(0xab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            TlvReaderDecodes(1, 2, serializedTlv, expectedTlv);
        }
        
        void TlvReaderDecodes2and1()
        {
            var serializedTlv = new byte[] { 0xab, 0x08, 0x05, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            var expectedTlv = new Tlv(0x8ab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            TlvReaderDecodes(2, 1, serializedTlv, expectedTlv);
        }
        
        void TlvReaderDecodes2and2()
        {
            var serializedTlv = new byte[] { 0xab, 0x02, 0x05, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            var expectedTlv = new Tlv(0x2ab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            TlvReaderDecodes(2, 2, serializedTlv, expectedTlv);
        }

        void TlvReaderDecodes(int typeLength, int sizeLength, byte[] serializedTlv, Tlv expectedTlv)
        {
            var parser = new TlvParser(typeLength, sizeLength);
            using(var memStream = new MemoryStream(serializedTlv))
            {
                var tlv = parser.Decode(memStream);
                Assert.Equal(expectedTlv, tlv);
            }
        }

        [Fact]
        public void TlvReaderReturnsNullOnEmptyStream()
        {
            var parser = new TlvParser(1, 1);
            using (var memStream = new MemoryStream(new byte[] { }))
            {
                var tlv = parser.Decode(memStream);
                Assert.Equal(null, tlv);
            }
        }

        [Fact]
        public void TlvReaderEncodesVarious()
        {
            TlvReaderEncodes1and1();
            TlvReaderEncodes2and1();
            TlvReaderEncodes1and2();
            TlvReaderEncodes2and2();
        }

        void TlvReaderEncodes1and1()
        {
            var tlv = new Tlv(0xab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            var expectedOutput = new byte[] { 0xab, 0x05, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            TlvReaderDecodes(1, 1, expectedOutput, tlv);
        }

        void TlvReaderEncodes1and2()
        {
            var tlv = new Tlv(0xab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            var expectedOutput = new byte[] { 0xab, 0x05, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            TlvReaderDecodes(1, 2, expectedOutput, tlv);
        }

        void TlvReaderEncodes2and1()
        {
            var tlv = new Tlv(0x8ab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            var expectedOutput = new byte[] { 0xab, 0x08, 0x05, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            TlvReaderDecodes(2, 1, expectedOutput, tlv);
        }

        void TlvReaderEncodes2and2()
        {
            var tlv = new Tlv(0x2ab, new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 });
            var expectedOutput = new byte[] { 0xab, 0x02, 0x05, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 };
            TlvReaderDecodes(2, 2, expectedOutput, tlv);
        }

        void TlvReaderEncodes(int typeLength, int sizeLength, byte[] expectedOutput, Tlv tlv)
        {
            var parser = new TlvParser(typeLength, sizeLength);
            using (var memStream = new MemoryStream())
            {
                parser.Encode(memStream, tlv);
                var output = memStream.ToArray();
                Assert.Equal(expectedOutput, output);
            }
        }
    }
}
