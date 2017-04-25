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
    public class BgpMessageParserTests
    {
        private readonly ITestOutputHelper output;

        public BgpMessageParserTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void BgpMessageEncodesToTlv()
        {
            // open message
            var serialisedOpenMessage = new byte[]
            {
                4, // BGP version 4
                0x30, 0x39, // AS 0x3039 = 12345
                0x00, 0xb4, // hold time = 180 seconds
                0x0a, 0x01, 0x02, 0x03, // identifier = 0x0a010203 or 10.1.2.3
                0 // no optional parameters
            };
            BgpMessageEncodeDecode(new Tlv(1, serialisedOpenMessage));

            // open message with optional params
            var serialisedOpenMessageWithParams = new byte[]
            {
                4, // BGP version 4
                0x30, 0x39, // AS 0x3039 = 12345
                0x00, 0xb4, // hold time = 180 seconds
                0x0a, 0x01, 0x02, 0x03, // identifier = 0x0a010203 or 10.1.2.3
                16, // 16 bytes of optional parameters
                0x02, 0x06, 0x01, 0x04, 0x00, 0x01, 0x00, 0x01, // capability, length 6
                0x02, 0x02, 0x80, 0x00, // capability, length 2
                0x02, 0x02, 0x02, 0x00, // capability, length 2
            };
            BgpMessageEncodeDecode(new Tlv(1, serialisedOpenMessageWithParams));
            // keepalive message
            BgpMessageEncodeDecode(new Tlv(3, new byte[] { }));
            // notification message "Unacceptable Hold Time"
            BgpMessageEncodeDecode(new Tlv(4, new byte[] { 2, 6 }));
        }

        void BgpMessageEncodeDecode(Tlv tlv)
        {
            var parser = new BgpMessageParser();
            var bgpMessage = parser.Decode(tlv);
            var outputTlv = parser.Encode(bgpMessage);
            Assert.Equal(tlv, outputTlv);
        }
    }
}
