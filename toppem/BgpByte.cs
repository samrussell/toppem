using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpByte : IPackable
    {
        public byte Value;

        public BgpByte(byte Value)
        {
            this.Value = Value;
        }

        public void Accept(IPackableVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as BgpByte;

            return (obj != null)
                && (Value == other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
