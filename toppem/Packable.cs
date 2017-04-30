using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class Packable
    {
        public IEnumerable<byte> Pack()
        {
            return FieldsForPacking(GetType()).Select(field => PackField(field))
                .Aggregate((IEnumerable<byte>) new List<byte>(), (sum, next) => sum.Concat(next));
        }

        static IOrderedEnumerable<FieldInfo> FieldsForPacking(Type t)
        {
            return t.GetFields()
                .Where(field => Attribute.IsDefined(field, typeof(FieldOrderAttribute)))
                .OrderBy(field => ((FieldOrderAttribute)field.GetCustomAttributes(typeof(FieldOrderAttribute), false)[0]).Order);
        }

        IEnumerable<byte> PackField(FieldInfo field)
        {
            dynamic data = field.GetValue(this);
            return PackData(data);
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

        public static T Build<T>(IEnumerable<byte> data) where T : Packable
        {
            var fields = FieldsForPacking(typeof(T));
            return (T)Activator.CreateInstance(typeof(T), GetArgs(fields, data));
        }

        public static object[] GetArgs(IOrderedEnumerable<FieldInfo> fields, IEnumerable<byte> data)
        {
            return null;
        }
    }
}
