using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PacksWithAttribute : Attribute
    {
        private Type _packer;

        public Type Packer { get { return _packer; } }

        public PacksWithAttribute(Type packer)
        {
            _packer = packer;
        }
    }
}
