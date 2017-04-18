using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class TlvParser
    {
        private int _typeLength;
        private int _sizeLength;
        private bool _networkByteOrder;

        public TlvParser(int typeLength, int sizeLength, bool networkByteOrder = false)
        {
            _typeLength = typeLength;
            _sizeLength = sizeLength;
            _networkByteOrder = networkByteOrder;
        }

        public Tlv Decode(Stream stream)
        {
            return new Tlv(ReadType(stream), ReadData(stream));
        }

        int ReadType(Stream stream)
        {
            return ReadNumber(stream, _typeLength);
        }

        byte[] ReadData(Stream stream)
        {
            var size = ReadNumber(stream, _sizeLength);
            byte[] output = new byte[size];
            int numBytesRead = 0;
            stream.Read(output, numBytesRead, size);

            return output;
        }

        int ReadNumber(Stream stream, int length)
        {
            byte[] input = new byte[length];
            int numBytesRead = 0;
            stream.Read(input, numBytesRead, length);

            if (_networkByteOrder)
                Array.Reverse(input);

            switch (length)
            {
                case 1:
                    return (int)input[0];
                case 2:
                    return BitConverter.ToInt16(input, 0);
                case 4:
                    return BitConverter.ToInt32(input, 0);
            }

            throw new Exception(@"sizeLength must be 1, 2 or 4");
        }
    }
}
