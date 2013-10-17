using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOCFactoryModel;
using IOCFactoryModel.Interface;
using IOCFactory.Util;

namespace IOCFactory.Model.Imp.InstCreator
{
    public class SingletonInstCreator : IInstCreator
    {
        internal SingletonInstCreator()
        {
        }

        public RegistCheckResult Check(RegistObjectContext context)
        {
            return true;
        }


        public object CreateInst(RegistObjectContext context, params object[] param)
        {
            if (context.Obj == null)
            {
                lock (context)
                {
                    var diCreator = InstCreatorFactory.Create(IOCFactoryModel.InstType.Normal);
                    context.Obj = diCreator.CreateInst(context, param);
                }
            }
            return context.Obj;
        }
    }
}
