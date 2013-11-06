using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOCFactoryModel.Interface;
using IOCFactory.Util;

namespace IOCFactory.Model.Imp.InstCreator
{
    public class DISingletonInstCreator : IInstCreator
    {
        private object _locker = new object();

        public IOCFactoryModel.RegistCheckResult Check(IOCFactoryModel.RegistObjectContext context)
        {
            var diCreator = InstCreatorFactory.Create(IOCFactoryModel.InstType.DI);
            return diCreator.Check(context);
        }

        public object CreateInst(IOCFactoryModel.RegistObjectContext context, params object[] param)
        {
            if (context.Obj == null)
            {
                lock (_locker)
                {
                    var diCreator = InstCreatorFactory.Create(IOCFactoryModel.InstType.DI);
                    context.Obj = diCreator.CreateInst(context, param);
                }
            }
            return context.Obj;
        }
    }
}
