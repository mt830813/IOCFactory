using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOCFactoryModel.Interface;
using IOCFactoryModel;

namespace IOCFactory.Model
{
    public class ObjectPool
    {
        private object[] pool;

        private IInstCreator creator;

        private int lastIndex;

        private int maxCount;

        private RegistObjectContext context;

        private Action<object> action;

        internal ObjectPool(int poolMaxCount, IInstCreator creator, RegistObjectContext context, Action<object> action)
        {
            pool = new object[poolMaxCount];
            this.maxCount = poolMaxCount;
            this.creator = creator;
            lastIndex = 0;
            this.context = context;
        }

        public object Get(params object[] param)
        {
            lastIndex = ++lastIndex % maxCount;

            if (pool[lastIndex] == null)
            {
                var obj = creator.CreateInst(context, param);
                pool[lastIndex] = obj;
                if (this.action != null)
                {
                    action(obj);
                }
                return obj;
            }
            return pool[lastIndex];
        }
    }
}
