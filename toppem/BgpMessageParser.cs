using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpMessageParser
    {
        public BgpMessage Decode(Tlv tlv)
        {
            return new BgpMessage();
        }

        public Tlv Encode(BgpMessage bgpMessage)
        {
            return new Tlv(1, new byte[] { });
        }
    }
}
