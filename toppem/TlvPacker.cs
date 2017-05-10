using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class TlvPacker : IPackFudge
    {
        private FudgePacker packer;

        public TlvPacker()
        {
            packer = new FudgePacker();
        }

        public IEnumerable<byte> Pack(object obj)
        {
            // use fudgepacker then add tlv stuff
            var packedInnerObject = packer.Pack(obj);

            var tlvAttribute = ((TlvAttribute)obj.GetType().GetCustomAttributes(typeof(TlvAttribute), false)[0]);
            var packedType = FudgeFactory.Packer(tlvAttribute.TypeLength).Pack(Convert.ChangeType(tlvAttribute.Type, tlvAttribute.TypeLength));
            var packedSize = FudgeFactory.Packer(tlvAttribute.SizeLength).Pack(Convert.ChangeType(packedInnerObject.Count(), tlvAttribute.SizeLength));

            return packedType.Concat(packedSize).Concat(packedInnerObject);
        }

        public object Unpack(Type type, Stream data)
        {
            // strip tlv stuff and use fudgepacker
            var tlvAttribute = ((TlvAttribute)type.GetCustomAttributes(typeof(TlvAttribute), false)[0]);

            var typeDef = FudgeFactory.Packer(tlvAttribute.TypeLength).Unpack(tlvAttribute.TypeLength, data);
            var size = FudgeFactory.Packer(tlvAttribute.SizeLength).Unpack(tlvAttribute.SizeLength, data);

            var innerObject = packer.Unpack(type, data);

            return innerObject;
        }
    }
}
