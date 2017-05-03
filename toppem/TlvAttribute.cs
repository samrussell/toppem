using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TlvAttribute : Attribute
    {
        private Type _typeLength;
        private Type _sizeLength;
        private int _type;

        public Type TypeLength { get { return _typeLength; } }
        public Type SizeLength { get { return _sizeLength; } }
        public int Type { get { return _type; } }

        public TlvAttribute(Type typeLength, Type sizeLength, int type)
        {
            _typeLength = typeLength;
            _sizeLength = sizeLength;
            _type = type;
        }
    }
}
