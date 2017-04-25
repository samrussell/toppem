using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class TlvParser : StreamParser
    {
        private int _typeLength;
        private int _sizeLength;

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

        void WriteType(Stream stream, int type)
        {
            WriteNumber(stream, _typeLength, type);
        }

        void WriteData(Stream stream, byte[] data)
        {
            WriteNumber(stream, _sizeLength, data.Length);
            stream.Write(data, 0, data.Length);
        }
    }
}
