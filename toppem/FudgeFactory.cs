using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class FudgeFactory
    {
        private static Dictionary<Type, IPackFudge> packers;
        private static Dictionary<Type, IPackFudge> packerInstances;

        static FudgeFactory()
        {
            packerInstances = new Dictionary<Type, IPackFudge>();
            packers = new Dictionary<Type, IPackFudge>
            {
                {typeof(uint), PackerInstance(typeof(CavemanPacker))},
                {typeof(ushort), PackerInstance(typeof(CavemanPacker))},
                {typeof(byte), PackerInstance(typeof(CavemanPacker))},
            };
        }

        public static void Register(Type classType)
        {
            var packerType = ((PacksWithAttribute)classType.GetCustomAttributes(typeof(PacksWithAttribute), false)[0]).PackerType;
            packers[classType] = PackerInstance(packerType);
        }

        private static IPackFudge PackerInstance(Type type)
        {
            if (packerInstances.ContainsKey(type))
            {
                return packerInstances[type];
            }
            else
            {
                IPackFudge packer = (IPackFudge)Activator.CreateInstance(type);
                packerInstances[type] = packer;
                return packer;
            }
        }

        public static IPackFudge Packer(Type type)
        {
            if (!packers.ContainsKey(type))
            {
                Register(type);
            }
            return packers[type];
        }
    }
}
