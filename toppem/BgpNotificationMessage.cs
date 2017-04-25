using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpNotificationMessage : IBgpMessage
    {
        public byte[] data;

        public BgpNotificationMessage(byte[] data)
        {
            this.data = data;
        }

        public void Accept(IBgpMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
