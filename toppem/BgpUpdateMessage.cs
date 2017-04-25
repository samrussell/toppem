using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpUpdateMessage : IBgpMessage
    {
        public IEnumerable<Prefix> prefixes;

        public BgpUpdateMessage(IEnumerable<Prefix> prefixes)
        {
            this.prefixes = prefixes;
        }

        public void Accept(IBgpMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
