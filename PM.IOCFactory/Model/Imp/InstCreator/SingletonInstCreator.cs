﻿using System;
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
        private object _locker = new object();

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
                lock (_locker)
                {
                    var diCreator = InstCreatorFactory.Create(IOCFactoryModel.InstType.Normal);
                    var obj = diCreator.CreateInst(context, param);
                    if (context.Obj == null)
                    {
                        context.Obj = obj;
                    }
                }
            }
            return context.Obj;
        }
    }
}
