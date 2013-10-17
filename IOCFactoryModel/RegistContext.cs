using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOCFactoryModel.Enum;
using IOCFactoryModel.Interface;

namespace IOCFactoryModel
{

    public class RegistObjectContext
    {
        public InstType InstType { get; set; }

        public Object Obj { get; set; }

        public Type ObjType { get; set; }

        public Dictionary<ContextParamNameEnum, object> Params { get; set; }

        public IInstCreator InstCreator { get; set; }

        public RegistObjectContext()
        {
            Params = new Dictionary<ContextParamNameEnum, object>();
        }

        public Type PType { get; set; }

        private int _hashCode;

        public int HashCode
        {
            get
            {
                if (_hashCode == 0)
                {
                    _hashCode = this.GetHashCode();
                }
                return _hashCode;
            }
        }
    }
}
