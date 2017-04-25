using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class Prefix
    {
        public byte[] prefix;
        public int bitLength;

        public Prefix(byte[] prefix, int bitLength)
        {
            this.prefix = prefix;
            this.bitLength = bitLength;
        }

        public static int LengthInBytes(int bitlength)
        {
            var remainder = bitlength % 8;
            var wholeBytesLength = bitlength / 8;

            return wholeBytesLength + (remainder > 0 ? 1 : 0);
        }
    }
}
