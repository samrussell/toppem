using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class StreamParser
    {
        protected bool _networkByteOrder = false;

        protected int ReadNumber(Stream stream, int length)
        {
            byte[] input = new byte[length];
            int numBytesRead = 0;
            stream.Read(input, numBytesRead, length);

            return DecodeNumber(input, length);
        }

        protected void WriteNumber(Stream stream, int length, int number)
        {
            stream.Write(EncodeNumber(number, length).ToArray(), 0, length);
        }

        protected int DecodeNumber(byte[] serializedNumber, int length)
        {
            if (_networkByteOrder)
                Array.Reverse(serializedNumber);

            switch (length)
            {
                case 1:
                    return (int)serializedNumber[0];
                case 2:
                    return BitConverter.ToInt16(serializedNumber, 0);
                case 4:
                    return BitConverter.ToInt32(serializedNumber, 0);
            }

            throw new Exception(@"sizeLength must be 1, 2 or 4");
        }

        protected IEnumerable<Byte> EncodeNumber(int number, int length)
        {
            byte[] serializedNumber = BitConverter.GetBytes(number);

            if (_networkByteOrder)
                Array.Reverse(serializedNumber);

            return serializedNumber.Take(length);
        }
    }
}
