using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public interface IPackFudge
    {
        IEnumerable<byte> Pack(object toPack);
        object Unpack(Type type, Stream data);
    }
}
