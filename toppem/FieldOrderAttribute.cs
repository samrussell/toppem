using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldOrderAttribute : Attribute
    {
        private int _order;

        public int Order { get { return _order; } }

        public FieldOrderAttribute(int order)
        {
            _order = order;
        }
    }
}
