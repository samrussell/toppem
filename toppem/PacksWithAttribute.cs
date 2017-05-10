using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacksWithAttribute : Attribute
    {
        private Type _packerType;

        public Type PackerType { get { return _packerType; } }

        public PacksWithAttribute(Type packerType)
        {
            _packerType = packerType;
        }
    }
}
