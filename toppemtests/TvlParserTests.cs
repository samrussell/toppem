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
        [Theory]
        [InlineData(new byte[] { 0xab, 0x05, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37}, 0xab, "12345")]
        public void TlvReaderDecodes1and1(byte[] input, int output_type, string output_data)
        {
            var memStream = new MemoryStream(input);
            var parser = new TlvParser(1, 1);
            var tlv = parser.Decode(memStream);
            Assert.Equal(output_type, tlv.Type);
            Assert.Equal(Encoding.ASCII.GetBytes(output_data), tlv.Data);
        }

        [Theory]
        [InlineData(new byte[] { 0xab, 0x05, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 }, 0xab, "12345")]
        public void TlvReaderDecodes1and2(byte[] input, int output_type, string output_data)
        {
            var memStream = new MemoryStream(input);
            var parser = new TlvParser(1, 2);
            var tlv = parser.Decode(memStream);
            Assert.Equal(output_type, tlv.Type);
            Assert.Equal(Encoding.ASCII.GetBytes(output_data), tlv.Data);
        }

        [Theory]
        [InlineData(new byte[] { 0xab, 0x05, 0x05, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 }, 0x5ab, "12345")]
        public void TlvReaderDecodes2and1(byte[] input, int output_type, string output_data)
        {
            var memStream = new MemoryStream(input);
            var parser = new TlvParser(2, 1);
            var tlv = parser.Decode(memStream);
            Assert.Equal(output_type, tlv.Type);
            Assert.Equal(Encoding.ASCII.GetBytes(output_data), tlv.Data);
        }

        [Theory]
        [InlineData(new byte[] { 0xab, 0x34, 0x05, 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37 }, 0x34ab, "12345")]
        public void TlvReaderDecodes2and2(byte[] input, int output_type, string output_data)
        {
            var memStream = new MemoryStream(input);
            var parser = new TlvParser(2, 2);
            var tlv = parser.Decode(memStream);
            Assert.Equal(output_type, tlv.Type);
            Assert.Equal(Encoding.ASCII.GetBytes(output_data), tlv.Data);
        }
    }
}
