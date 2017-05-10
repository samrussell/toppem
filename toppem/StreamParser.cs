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

        protected uint ReadNumber(Stream stream, uint length)
        {
            byte[] input = new byte[length];
            int numBytesRead = 0;
            stream.Read(input, numBytesRead, Convert.ToInt32(length));

            return DecodeNumber(input, length);
        }

        protected void WriteNumber(Stream stream, uint length, uint number)
        {
            stream.Write(EncodeNumber(number, length).ToArray(), 0, Convert.ToInt32(length));
        }

        protected uint DecodeNumber(byte[] serializedNumber, uint length)
        {
            if (_networkByteOrder)
                Array.Reverse(serializedNumber);

            switch (length)
            {
                case 1:
                    return (uint)serializedNumber[0];
                case 2:
                    return BitConverter.ToUInt16(serializedNumber, 0);
                case 4:
                    return BitConverter.ToUInt32(serializedNumber, 0);
            }

            throw new Exception(@"sizeLength must be 1, 2 or 4");
        }

        protected IEnumerable<Byte> EncodeNumber(uint number, uint length)
        {
            byte[] serializedNumber;
            switch (length)
            {
                case 1:
                    serializedNumber = new byte[] { Convert.ToByte(number) };
                    break;
                case 2:
                    serializedNumber = BitConverter.GetBytes(Convert.ToUInt16(number));
                    break;
                case 4:
                    serializedNumber = BitConverter.GetBytes(Convert.ToUInt32(number));
                    break;
                default:
                    throw new Exception(@"sizeLength must be 1, 2 or 4");
            }

            if (_networkByteOrder)
                Array.Reverse(serializedNumber);

            return serializedNumber.Take(Convert.ToInt32(length));
        }
    }
}
