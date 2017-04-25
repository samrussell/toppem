using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpMessageEncoder : IBgpMessageVisitor
    {
        public Tlv tlv = null;

        public void Visit(BgpKeepaliveMessage keepaliveMessage)
        {
            tlv = new Tlv(3, new byte[] { });
        }

        public void Visit(BgpOpenMessage openMessage)
        {
            throw new Exception("BgpOpenMessage not supported");
        }

        public void Visit(BgpNotificationMessage notificationMessage)
        {
            tlv = new Tlv(4, notificationMessage.data);
        }
    }
}
