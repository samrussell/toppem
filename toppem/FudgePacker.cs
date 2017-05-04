using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class FudgePacker
    {
        public IEnumerable<byte> Pack(object obj)
        {
            var serialisedData = FieldsForPacking(obj.GetType()).Select(field => PackField(obj, field))
                .Aggregate((IEnumerable<byte>) new List<byte>(), (sum, next) => sum.Concat(next));

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
            else if(Attribute.IsDefined(field, typeof(PacksWithAttribute)))
            {
                var packer = ((PacksWithAttribute)field.GetCustomAttributes(typeof(PacksWithAttribute), false)[0]).Packer;
                var method = packer.GetMethod("Pack", new Type[] { data.GetType() });
                if (method == null)
                {
                    throw new Exception ("Couldn't find packer.Pack for " + data.GetType().ToString());
                }
                return (IEnumerable<byte>) method.Invoke(null, new object[] { data });
            }
            else
            {
                throw new Exception("Cannot pack field " + field.Name.ToString());
            }
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
            var args = GetArgs(fields, stream);
            return Activator.CreateInstance(type, args);
        }

        public object[] GetArgs(IOrderedEnumerable<FieldInfo> fields, Stream stream)
        {
            return fields.Select(field => UnpackField(field, stream)).ToArray();
        }

        object UnpackField(FieldInfo field, Stream stream)
        {
            if (Attribute.IsDefined(field, typeof(PackableAttribute)))
            {
                return new FudgePacker().UnpackFromStream(field.FieldType, stream);
            }
            else if (Attribute.IsDefined(field, typeof(PacksWithAttribute)))
            {
                var packer = ((PacksWithAttribute)field.GetCustomAttributes(typeof(PacksWithAttribute), false)[0]).Packer;
                var method = packer.GetMethod("Unpack", new Type[] { typeof(Type), typeof(Stream) });
                if (method == null)
                {
                    throw new Exception("Couldn't find packer.Pack for " + field.FieldType.ToString());
                }
                return method.Invoke(null, new object[] { field.FieldType, stream });
            }
            else
            {
                throw new Exception("Cannot pack field " + field.Name.ToString());
            }
        }
        
        private static readonly Dictionary<Type, Func<FudgePacker, Stream, object>> DelegateMap = new Dictionary<Type, Func<FudgePacker, Stream, object>>
        {
            {typeof(uint), (x, y) => x.UnpackInt(y)},
            {typeof(ushort), (x, y) => x.UnpackShort(y)},
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
            return (object)BitConverter.ToUInt16(buffer, 0);
        }

        object UnpackInt(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            Array.Reverse(buffer);
            return (object)BitConverter.ToUInt32(buffer, 0);
        }
    }
}
