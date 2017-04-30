using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpOpenMessage : Packable, IBgpMessage
    {
        [FieldOrder(1)]
        public byte version;
        [FieldOrder(2)]
        public short asNum;
        [FieldOrder(3)]
        public short holdTime;
        [FieldOrder(4)]
        public int identifier;
        public IEnumerable<Tlv> capabilities;

        public BgpOpenMessage(int version, int asNum, int holdTime, int identifier, IEnumerable<Tlv> capabilities)
        {
            this.version = Convert.ToByte(version);
            this.asNum = Convert.ToInt16(asNum);
            this.holdTime = Convert.ToInt16(holdTime);
            this.identifier = identifier;
            this.capabilities = capabilities;
        }

        public void Accept(IBgpMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
