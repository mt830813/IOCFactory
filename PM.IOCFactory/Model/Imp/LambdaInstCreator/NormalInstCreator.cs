using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOCFactoryModel;
using IOCFactoryModel.Interface;
using System.Linq.Expressions;
using System.Reflection;

namespace IOCFactory.Model.Imp.LambdaInstCreator
{


    public class NormalInstCreator : IInstCreator
    {
        private object _locker = new object();

        internal delegate object ObjectActivator(params object[] args);

        private static ObjectActivator GetActivator(Type t, ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivator), newExp, param);

            //compile it
            ObjectActivator compiled = (ObjectActivator)lambda.Compile();
            return compiled;
        }

        private volatile Dictionary<int, ObjectActivator> dicCache;

        internal NormalInstCreator()
        {
            dicCache = new Dictionary<int, ObjectActivator>();
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

            ObjectActivator objectCreater;
            try
            {
                objectCreater = dicCache[context.HashCode];
            }
            catch (KeyNotFoundException)
            {

                try
                {
                    var constructor = context.ObjType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, types, null);
                    dicCache.Add(context.HashCode, NormalInstCreator.GetActivator(context.ObjType, constructor));
                    objectCreater = dicCache[context.HashCode];
                }
                catch (ArgumentException ex)
                {
                    objectCreater = dicCache[context.HashCode];
                }
            }
            return objectCreater(param);
        }
    }
}
