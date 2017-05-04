using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class CavemanPacker
    {
        private static readonly Dictionary<Type, Func<Stream, object>> DelegateMap = new Dictionary<Type, Func<Stream, object>>
        {
            {typeof(uint), x => CavemanPacker.UnpackInt(x)},
            {typeof(ushort), x => CavemanPacker.UnpackShort(x)},
            {typeof(byte), x => CavemanPacker.UnpackByte(x)},
        };

        public static IEnumerable<byte> Pack(byte input)
        {
            var x = (byte)0xff;
            var y = (byte)0xfe;
            byte gary = (byte)(x & y);
            return new List<byte> {
                (byte)(input & 0xff)
            };
        }

        public static IEnumerable<byte> Pack(ushort input)
        {
            return new List<ushort> {
                (ushort)((input >> 8) & 0xff),
                (ushort)(input & 0xff)
            }.Select(x => Convert.ToByte(x));
        }

        public static IEnumerable<byte> Pack(uint input)
        {
            return new List<uint> {
                ((input >> 24) & 0xff),
                ((input >> 16) & 0xff),
                ((input >> 8) & 0xff),
                (input & 0xff)
            }.Select(x => Convert.ToByte(x));
        }

        public static T Unpack<T>(Stream stream)
        {
            return (T)DelegateMap[typeof(T)](stream);
        }

        public static object Unpack(Type type, Stream stream)
        {
            return DelegateMap[type](stream);
        }

        public static uint UnpackInt(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            Array.Reverse(buffer);

            return BitConverter.ToUInt32(buffer, 0);
        }

        public static ushort UnpackShort(Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            Array.Reverse(buffer);

            return BitConverter.ToUInt16(buffer, 0);
        }

        public static byte UnpackByte(Stream stream)
        {
            var buffer = new byte[1];
            stream.Read(buffer, 0, 1);

            return buffer[0];
        }
    }
}
