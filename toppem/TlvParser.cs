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

        public void Encode(Stream stream, Tlv tlv)
        {
            WriteType(stream, tlv.Type);
            WriteData(stream, tlv.Data);
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

            return DecodeNumber(input, length);
        }

        void WriteType(Stream stream, int type)
        {
            WriteNumber(stream, _typeLength, type);
        }

        void WriteData(Stream stream, byte[] data)
        {
            WriteNumber(stream, _sizeLength, data.Length);
            stream.Write(data, 0, data.Length);
        }

        void WriteNumber(Stream stream, int length, int number)
        {
            stream.Write(EncodeNumber(number, length), 0, length);
        }

        int DecodeNumber(byte[] serializedNumber, int length)
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

        byte[] EncodeNumber(int number, int length)
        {
            byte[] serializedNumber = BitConverter.GetBytes(number);

            if (_networkByteOrder)
                Array.Reverse(serializedNumber);

            return serializedNumber;
        }
    }
}
