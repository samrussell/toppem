﻿using System;
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
            return FieldsForPacking(obj.GetType()).Select(field => PackField(obj, field))
                .Aggregate((IEnumerable<byte>) new List<byte>(), (sum, next) => sum.Concat(next));
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
                return new BgpPacker().Pack(data);
            }
            else
            {
                return PackData(data);
            }
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
            var stream = new MemoryStream(data.ToArray());
            return UnpackFromStream(type, stream);
        }

        public object UnpackFromStream(Type type, Stream stream)
        {
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
                dynamic emptyField = Activator.CreateInstance(field.FieldType);
                return UnpackData(emptyField, stream);
            }
        }

        object UnpackData(byte _, Stream stream)
        {
            var buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            return (object)buffer[0];
        }

        object UnpackData(short _, Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            Array.Reverse(buffer);
            return (object)BitConverter.ToInt16(buffer, 0);
        }

        object UnpackData(int _, Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            Array.Reverse(buffer);
            return (object)BitConverter.ToInt32(buffer, 0);
        }
    }
}