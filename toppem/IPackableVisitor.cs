using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public interface IPackableVisitor
    {
        void Visit(BgpByte packable);
        void Visit(BgpShort packable);
        void Visit(BgpInt packable);
    }
}
