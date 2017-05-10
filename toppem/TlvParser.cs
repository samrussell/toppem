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
        private uint _typeLength;
        private uint _sizeLength;

        public TlvParser(uint typeLength, uint sizeLength, bool networkByteOrder = false)
        {
            _typeLength = typeLength;
            _sizeLength = sizeLength;
            _networkByteOrder = networkByteOrder;
        }

        public Tlv Decode(Stream stream)
        {
            if (stream.Length == stream.Position)
            {
                return null;
            }

            return new Tlv(ReadType(stream), ReadData(stream));
        }

        public void Encode(Stream stream, Tlv tlv)
        {
            WriteType(stream, tlv.Type);
            WriteData(stream, tlv.Data);
        }

        uint ReadType(Stream stream)
        {
            return ReadNumber(stream, _typeLength);
        }

        byte[] ReadData(Stream stream)
        {
            var size = ReadNumber(stream, _sizeLength);
            byte[] output = new byte[size];
            int numBytesRead = 0;
            stream.Read(output, numBytesRead, Convert.ToInt32(size));

            return output;
        }

        void WriteType(Stream stream, uint type)
        {
            WriteNumber(stream, _typeLength, type);
        }

        void WriteData(Stream stream, byte[] data)
        {
            WriteNumber(stream, _sizeLength, Convert.ToUInt32(data.Length));
            stream.Write(data, 0, data.Length);
        }
    }
}
