using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class Tlv
    {
        public int Type { get; set; }
        public byte[] Data { get; set; }

        public Tlv(int type, byte[] data)
        {
            this.Type = type;
            this.Data = data;
        }
    }
}
