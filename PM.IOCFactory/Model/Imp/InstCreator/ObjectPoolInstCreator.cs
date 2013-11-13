using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOCFactoryModel.Interface;
using IOCFactory.Util;

namespace IOCFactory.Model.Imp.InstCreator
{
    public class ObjectPoolInstCreator : IInstCreator
    {
        private Dictionary<int, ObjectPool> pools;

        internal ObjectPoolInstCreator()
        {
            pools = new Dictionary<int, ObjectPool>();
        }

        public IOCFactoryModel.RegistCheckResult Check(IOCFactoryModel.RegistObjectContext context)
        {
            return true;
        }

        public object CreateInst(IOCFactoryModel.RegistObjectContext context, params object[] param)
        {
            var poolMaxCount = int.Parse(context.Params[IOCFactoryModel.Enum.ContextParamNameEnum.POOL_MAXCOUNT].ToString());

            Action<object> action = (Action<object>)context.Params[IOCFactoryModel.Enum.ContextParamNameEnum.POOL_INITACTION];

            ObjectPool pool;

            try
            {
                pool = pools[context.HashCode];
            }
            catch (KeyNotFoundException)
            {
                pools.Add(context.HashCode, new ObjectPool(poolMaxCount, InstCreatorFactory.Create(IOCFactoryModel.InstType.Normal), context, action));
                pool = pools[context.HashCode];
            }

            return pool.Get(param);
        }
    }
}
