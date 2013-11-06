using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using IOCFactoryModel;
using IOCFactoryModel.Interface;

namespace IOCFactory.Model.Imp.ILInstCreator
{
    public class NormalInstCreator : IInstCreator
    {
        private delegate object ObjectActivator(params object[] args);

        private static ObjectActivator GetActivator(Type t, ConstructorInfo ctor)
        {

            DynamicMethod method = new DynamicMethod("CreateIntance", t,
    new Type[] { typeof(object[]) });
            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);//arr
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ldelem_Ref);
            gen.Emit(OpCodes.Unbox_Any, typeof(int));
            gen.Emit(OpCodes.Ldarg_0);//arr
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ldelem_Ref);
            gen.Emit(OpCodes.Castclass, typeof(string));
            gen.Emit(OpCodes.Newobj, ctor);// new Created
            gen.Emit(OpCodes.Ret);
            return (ObjectActivator)method.CreateDelegate(typeof(ObjectActivator));
        }

        private volatile Dictionary<Type, ObjectActivator> dicCache;

        internal NormalInstCreator()
        {
            dicCache = new Dictionary<Type, ObjectActivator>();
        }

        public RegistCheckResult Check(RegistObjectContext context)
        {
            return true;
        }


        public object CreateInst(RegistObjectContext context, params object[] param)
        {
            var types = new Type[param.Length];

            for (int i = 0; i < param.Length; i++)
            {
                types[i] = param[i].GetType();
            }

            var constructor = context.ObjType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, types, null);
            if (!dicCache.ContainsKey(context.ObjType))
            {
                dicCache.Add(context.ObjType, NormalInstCreator.GetActivator(context.ObjType, constructor));
            }
            var objectCreater = dicCache[context.ObjType];

            return objectCreater(param.ToArray<Object>());
        }

    }
}
