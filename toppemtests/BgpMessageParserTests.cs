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
        public void BgpMessageDecodesToTlv()
        {
            var inputTlv = new Tlv(1, new byte[] { });
            var parser = new BgpMessageParser();
            var bgpMessage = parser.Decode(inputTlv);
            var outputTlv = parser.Encode(bgpMessage);
            
            Assert.Equal(inputTlv, outputTlv);
        }
    }
}
