using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    [PacksWith(typeof(FudgePacker))]
    public class BgpOpenMessage : IBgpMessage
    {
        [FieldOrder(1)]
        public byte version;
        [FieldOrder(2)]
        public ushort asNum;
        [FieldOrder(3)]
        public ushort holdTime;
        [FieldOrder(4)]
        public uint identifier;
        public IEnumerable<Tlv> capabilities;

        public BgpOpenMessage(uint version, uint asNum, uint holdTime, uint identifier, IEnumerable<Tlv> capabilities)
        {
            this.version = Convert.ToByte(version);
            this.asNum = Convert.ToUInt16(asNum);
            this.holdTime = Convert.ToUInt16(holdTime);
            this.identifier = identifier;
            this.capabilities = capabilities;
        }

        public void Accept(IBgpMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
