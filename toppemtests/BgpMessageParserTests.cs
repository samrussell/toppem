using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toppem;
using Xunit;

namespace toppemtests
{
    public class BgpMessageParserTests
    {
        [Fact]
        public void DecodeBgpKeepaliveMessage()
        {
            var parser = new BgpMessageParser();
            var keepAlive = new BgpKeepaliveMessage();
            var tlv = new Tlv(3, new byte[] { });

            Assert.Equal(keepAlive, parser.Decode(tlv));
        }

        [Fact]
        public void EncodeBgpKeepaliveMessage()
        {
            var parser = new BgpMessageParser();
            var keepAlive = new BgpKeepaliveMessage();
            var tlv = new Tlv(3, new byte[] { });

            Assert.Equal(tlv, parser.Encode(keepAlive));
        }

        [Fact]
        public void BgpMessageEncodesToTlv()
        {
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
