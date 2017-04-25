using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpMessageEncoder : StreamParser, IBgpMessageVisitor
    {
        public Tlv tlv = null;

        public void Visit(BgpOpenMessage openMessage)
        {
            var data = EncodeNumber(openMessage.version, 1).
                Concat(EncodeNumber(openMessage.asNum, 2)).
                Concat(EncodeNumber(openMessage.holdTime, 2)).
                Concat(EncodeNumber(openMessage.identifier, 4)).
                Concat(EncodeNumber(0, 1));
            tlv = new Tlv(1, data.ToArray());
        }

        public void Visit(BgpKeepaliveMessage keepaliveMessage)
        {
            tlv = new Tlv(3, new byte[] { });
        }

        public void Visit(BgpNotificationMessage notificationMessage)
        {
            tlv = new Tlv(4, notificationMessage.data);
        }
    }
}
