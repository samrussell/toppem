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
        public byte[] Pack()
        {
            return GetType().GetFields().OrderBy(field => ((FieldOrderAttribute)field.GetCustomAttributes(typeof(FieldOrderAttribute), false)[0]).Order).Select(field => PackField(field)).Aggregate((IEnumerable<byte>) new List<byte>(), (sum, next) => sum.Concat(next)).ToArray();
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
    }
}
