using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpOpenMessage : IBgpMessage
    {
        public int version;
        public int asNum;
        public int holdTime;
        public int identifier;

        public BgpOpenMessage(int version, int asNum, int holdTime, int identifier)
        {
            this.version = version;
            this.asNum = asNum;
            this.holdTime = holdTime;
            this.identifier = identifier;
        }

        public void Accept(IBgpMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
