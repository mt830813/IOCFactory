using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOCFactoryModel.Interface;
using System.Reflection;
using IOCFactoryModel;
using IOCFactory.Util;

namespace IOCFactory.Model.Imp.InstCreator
{
    public class DIInstCreator : IInstCreator
    {
        private Dictionary<Type, Type[]> cache;

        internal DIInstCreator()
        {
            cache = new Dictionary<Type, Type[]>();
        }

        private bool HasCycle(RegistObjectContext context)
        {

            return false;
        }

        public RegistCheckResult Check(IOCFactoryModel.RegistObjectContext context)
        {
            var returnValue = new RegistCheckResult();
            returnValue.IsPass = false;

            var objType = context.ObjType;

            ConstructorInfo[] constructs = objType.GetConstructors();
            if (constructs.Length != 1)
            {
                returnValue.Message = string.Format("regist as DI Inst must and only have 1 construct method");
                return returnValue;
            }

            var construct = constructs[0];

            var pArray = construct.GetParameters();

            foreach (var p in pArray)
            {
                if (!p.ParameterType.IsInterface)
                {
                    returnValue.Message = string.Format("regsit as DI Inst the construct method's paramters must be interface type");
                    return returnValue;
                }
            }
            return true;
        }


        public object CreateInst(RegistObjectContext context, params object[] param)
        {
            var objType = context.ObjType;

            var index = 0;

            if (!this.cache.ContainsKey(objType))
            {
                lock (this.cache)
                {
                    Type[] list;

                    ConstructorInfo[] constructs = objType.GetConstructors();
                    var construct = constructs[0];
                    var pArray = construct.GetParameters();
                    list = new Type[pArray.Length];

                    foreach (var p in pArray)
                    {
                        list[index] = p.ParameterType;
                        index++;
                    }
                    this.cache.Add(objType, list);
                }
            }

            Type[] types = this.cache[objType];

            var factory = Factory.GetInst();

            var inPArray = new object[types.Length];
            index = 0;
            foreach (Type t in types)
            {
                inPArray[index] = factory.Get(t);
                index++;
            }

            var diCreator = InstCreatorFactory.Create(IOCFactoryModel.InstType.Normal);

            var returnValue = diCreator.CreateInst(context, inPArray);
            return returnValue;
        }
    }
}
