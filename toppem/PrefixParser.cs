using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class PrefixParser : StreamParser
    {
        public Prefix Decode(Stream stream)
        {
            if (stream.Length == stream.Position)
            {
                return null;
            }

            var bitLength = ReadNumber(stream, 1);
            return new Prefix(ReadPrefix(stream, bitLength), bitLength);
        }

        public void Encode(Stream stream, Prefix prefix)
        {
            WriteNumber(stream, 1, prefix.bitLength);
            WritePrefix(stream, prefix.prefix);
        }

        byte[] ReadPrefix(Stream stream, int bitLength)
        {
            var length = Prefix.LengthInBytes(bitLength);
            byte[] output = new byte[length];
            int numBytesRead = 0;
            stream.Read(output, numBytesRead, length);

            return output;
        }

        void WritePrefix(Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
    }
}
