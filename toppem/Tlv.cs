using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class Tlv
    {
        public uint Type { get; set; }
        public byte[] Data { get; set; }

        public Tlv(uint type, byte[] data)
        {
            this.Type = type;
            this.Data = data;
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            var tlv = obj as Tlv;

            return (tlv != null)
                && (Type == tlv.Type)
                && (Data.SequenceEqual(tlv.Data));
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Data.GetHashCode();
        }
    }
}
