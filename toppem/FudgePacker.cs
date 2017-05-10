using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class FudgePacker : IPackFudge
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
            
            return FudgeFactory.Packer(field.FieldType).Pack(data);
        }

        public object Unpack(Type type, IEnumerable<byte> data)
        {
            using (var stream = new MemoryStream(data.ToArray()))
            {
                return Unpack(type, stream);
            }
        }

        public object Unpack(Type type, Stream stream)
        {
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
            return FudgeFactory.Packer(field.FieldType).Unpack(field.FieldType, stream);
        }
    }
}
