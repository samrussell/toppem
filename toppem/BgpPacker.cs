using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpPacker
    {
        public IEnumerable<byte> Pack(object obj)
        {
            var serialisedData = FieldsForPacking(obj.GetType()).Select(field => PackField(obj, field))
                .Aggregate((IEnumerable<byte>) new List<byte>(), (sum, next) => sum.Concat(next));

            if(Attribute.IsDefined(obj.GetType(), typeof(TlvAttribute)))
            {
                var tlvAttributes = ((TlvAttribute)obj.GetType().GetCustomAttributes(typeof(TlvAttribute), false)[0]);
                IEnumerable<byte> serialisedType = PackData((dynamic) Convert.ChangeType(tlvAttributes.Type, tlvAttributes.TypeLength));
                IEnumerable<byte> serialisedLength = PackData((dynamic)Convert.ChangeType(serialisedData.Count(), tlvAttributes.SizeLength));
                return serialisedType.Concat(serialisedLength).Concat(serialisedData);
            }

            return serialisedData;
        }

        static IOrderedEnumerable<FieldInfo> FieldsForPacking(Type t)
        {
            return t.GetFields()
                .Where(field => Attribute.IsDefined(field, typeof(FieldOrderAttribute)))
                .OrderBy(field => ((FieldOrderAttribute)field.GetCustomAttributes(typeof(FieldOrderAttribute), false)[0]).Order);
        }

        IEnumerable<byte> PackField(object obj, FieldInfo field)
        {
            dynamic data = field.GetValue(obj);

            if (Attribute.IsDefined(field, typeof(PackableAttribute)))
            {
                return Pack(data);
            }
            else
            {
                return PackData(data);
            }
        }

        IEnumerable<byte> PackData(ZeroLength data)
        {
            return new List<byte> {};
        }

        IEnumerable<byte> PackData(byte data)
        {
            return new List<byte> { data };
        }

        IEnumerable<byte> PackData(short data)
        {
            return new List<int> { ((data >> 8) & 0xff), (data & 0xff) }.Select(x => Convert.ToByte(x));
        }

        IEnumerable<byte> PackData(int data)
        {
            return new List<int> {
                ((data >> 24) & 0xff),
                ((data >> 16) & 0xff),
                ((data >> 8) & 0xff),
                (data & 0xff)
            }.Select(x => Convert.ToByte(x));
        }

        public object Unpack(Type type, IEnumerable<byte> data)
        {
            using (var stream = new MemoryStream(data.ToArray()))
            {
                return UnpackFromStream(type, stream);
            }
        }

        public object UnpackFromStream(Type type, Stream stream)
        {
            if (Attribute.IsDefined(type, typeof(TlvAttribute)))
            {
                var tlvAttributes = ((TlvAttribute)type.GetCustomAttributes(typeof(TlvAttribute), false)[0]);
                var tlvType = DelegateMap[tlvAttributes.TypeLength](this, stream);
                var tlvLength = DelegateMap[tlvAttributes.SizeLength](this, stream);
            }

            var fields = FieldsForPacking(type);
            return Activator.CreateInstance(type, GetArgs(fields, stream));
        }

        public object[] GetArgs(IOrderedEnumerable<FieldInfo> fields, Stream stream)
        {
            return fields.Select(field => UnpackField(field, stream)).ToArray();
        }

        object UnpackField(FieldInfo field, Stream stream)
        {
            if (Attribute.IsDefined(field, typeof(PackableAttribute)))
            {
                return new BgpPacker().UnpackFromStream(field.FieldType, stream);
            }
            else
            {
                return DelegateMap[field.FieldType](this, stream);
            }
        }
        
        private static readonly Dictionary<Type, Func<BgpPacker, Stream, object>> DelegateMap = new Dictionary<Type, Func<BgpPacker, Stream, object>>
        {
            {typeof(int), (x, y) => x.UnpackInt(y)},
            {typeof(short), (x, y) => x.UnpackShort(y)},
            {typeof(byte), (x, y) => x.UnpackByte(y)},
            {typeof(ZeroLength), (x, y) => x.UnpackZeroLength(y)},
        };

        object UnpackZeroLength(Stream stream)
        {
            return null;
        }

        object UnpackByte(Stream stream)
        {
            var buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            return (object)buffer[0];
        }

        object UnpackShort(Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            Array.Reverse(buffer);
            return (object)BitConverter.ToInt16(buffer, 0);
        }

        object UnpackInt(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            Array.Reverse(buffer);
            return (object)BitConverter.ToInt32(buffer, 0);
        }
    }
}
