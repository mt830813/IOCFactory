using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOCFactory.Util;
using IOCFactoryModel;
using IOCFactoryModel.Enum;
using IOCFactoryModel.Interface;
using System.IO;
using IOCFactory.Model.Imp.RegistContextReader;

namespace IOCFactory
{
    internal class CustomerGetMethodContext
    {
        public Func<Type, bool> CheckMethod { get; set; }

        public Func<Type, object> GetMethod { get; set; }

        public CustomerMethodEffectEnum EffectEnum { get; set; }
    }

    public class Factory
    {

        private const string DEFAULTNAME = "default";

        private Dictionary<Type, Dictionary<string, RegistObjectContext>> dic;

        private delegate T customerGetMethodDelegate<T>();

        private List<CustomerGetMethodContext> customerGetMethodDic;

        private static Factory factory;

        public static Factory GetInst()
        {
            if (factory == null)
            {
                factory = new Factory();
            }
            return factory;
        }

        private Factory()
        {
            dic = new Dictionary<Type, Dictionary<string, RegistObjectContext>>();
            customerGetMethodDic = new List<CustomerGetMethodContext>();
        }


        public void RegistFromFile(string filePath, FactoryMappingFilePattern pattern = FactoryMappingFilePattern.Normal)
        {
            if (File.Exists(filePath))
            {
                var contextReader = InstCreatorFactory.GetReader(pattern);

                var contexts = contextReader.GetMappingContexts(filePath);

                foreach (var context in contexts.Contexts)
                {
                    this.Regist(context.PType, context.CType, context.InstType, context.Name);
                }

            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qType"></param>
        /// <param name="instType"></param>
        public void Regist<T>(Type qType, InstType instType)
        {
            this.Regist<T>(qType, instType, DEFAULTNAME);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qType"></param>
        /// <param name="instType"></param>
        /// <param name="name"></param>
        public void Regist<T>(Type qType, InstType instType, string name)
        {
            this.Regist(typeof(T), qType, instType, name);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="cType"></param>
        /// <param name="instType"></param>
        public void Regist(Type pType, Type cType, InstType instType)
        {
            this.Regist(pType, cType, instType, DEFAULTNAME);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="cType"></param>
        /// <param name="instType"></param>
        /// <param name="name"></param>
        public void Regist(Type pType, Type cType, InstType instType, string name)
        {
            Dictionary<string, RegistObjectContext> cDic = null;
            lock (dic)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = DEFAULTNAME;
                }
                try
                {
                    cDic = dic[pType];
                }
                catch (KeyNotFoundException)
                {
                    dic.Add(pType, new Dictionary<string, RegistObjectContext>());
                    cDic = dic[pType];
                }

                if (instType == InstType.Decorate)
                {
                    throw new Exception(string.Format("Decorate Inst Not Allow Use Normal Regist Method To Regist"));
                }

                if (cDic.ContainsKey(name))
                {
                    throw new Exception(string.Format("Regist Type '{0}' Must be Unique", name));
                }
                else
                {
                    lock (cDic)
                    {
                        RegistObjectContext context = new RegistObjectContext();

                        context.ObjType = cType;
                        context.PType = pType;

                        this.SetContextInstType(instType, context);
                        cDic.Add(name, context);
                    }
                }
            }
        }

        /// <summary>
        /// 注册 自定义获取方法
        /// 用于整合多处IOC工厂
        /// 慎用：任何注册 永久的的应用范围后 将会对Factory的Get 造成永久的效率影响，所以使用时请慎重。
        /// </summary>
        /// <param name="checkMethod">用于验证 那些类型 由此自定义 方法获取 Type:接口类型,true:将由getMethod进行返回</param>
        /// <param name="getMethod">自定义方法的 实例获取方法 Type:接口类型 object:返回的类型 注意请返回接口对应的实现类型，否则将会抛出异常</param>
        /// <param name="expectEffectRange">期待此方法的应用范围  </param>        
        public void RegistCustomerGetFunc(Func<Type, bool> checkMethod, Func<Type, object> getMethod, CustomerMethodEffectEnum expectEffectRange)
        {
            customerGetMethodDic.Add(
                new CustomerGetMethodContext()
                {
                    CheckMethod = checkMethod,
                    GetMethod = getMethod,
                    EffectEnum = expectEffectRange
                });
        }

        /// <summary>
        /// 注册 
        /// </summary>
        /// <typeparam name="T">基类或接口</typeparam>
        /// <typeparam name="Q">实现类</typeparam>
        /// <param name="instType">类实例化方式</param>
        public void Regist<T, Q>(InstType instType) where Q : T
        {
            Regist<T, Q>(DEFAULTNAME, instType);
        }

        /// <summary>
        /// 注册 
        /// 不允许使用装饰者作为实例类型进行注册，否则会抛出异常
        /// </summary>
        /// <typeparam name="T">基类或接口</typeparam>
        /// <typeparam name="Q">实现类</typeparam>
        /// <param name="instType">类实例化方式</param>
        /// <param name="name">注册对象</param>
        public void Regist<T, Q>(string name, InstType instType)
            where Q : T
        {
            this.Regist<T>(typeof(Q), instType, name);
        }

        /// <summary>
        /// 以装饰者模式进行注册        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="name">注册后的关键字</param>
        public void RegistDecorate<T, Q>(string name) where Q : T
        {
            RegistDecorate<T, Q>(name, DEFAULTNAME);
        }


        /// <summary>
        /// 以装饰者模式进行注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="name">注册后的关键字</param>
        /// <param name="toDecorateName">要装饰的已注册的对象</param>
        public void RegistDecorate<T, Q>(string name, string toDecorateName) where Q : T
        {
            if (!IsRegist<T>(name))
            {
                Regist<T, Q>(name, InstType.Normal);
            }
            var context = GetContext(typeof(T), name);

            this.SetContextInstType(InstType.Decorate, context);

            if (!context.Params.ContainsKey(ContextParamNameEnum.INSTCHAIN))
            {
                context.Params.Add(ContextParamNameEnum.INSTCHAIN, new List<Type>());
                context.Params.Add(ContextParamNameEnum.TODECORATENAME, toDecorateName);
            }
            List<Type> chainList = (List<Type>)context.Params[ContextParamNameEnum.INSTCHAIN];
            chainList.Add(typeof(Q));
        }


        /// <summary>
        /// 根据基类 获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(params object[] param)
        {
            return this.Get<T>(DEFAULTNAME, param);
        }

        /// <summary>
        /// 根据基类 以及注册名获取对象
        /// 如果没有 返回 默认注册对象
        /// 如果还没有 将会抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name, params object[] param)
        {
            try
            {
                return (T)this.Get(typeof(T), name, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 判断 在基类下是否已经注册该名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsRegist<T>(string name)
        {
            bool returnValue = true;
            if (!this.dic.ContainsKey(typeof(T)))
            {
                returnValue = false;
                return returnValue;
            }
            var cDic = dic[typeof(T)];
            if (!cDic.ContainsKey(name))
            {
                return false;
            }
            return returnValue;
        }


#if DEBUG
        /// <summary>
        /// debug 时才可以使用。        
        /// </summary>
        public void Clear()
        {
            this.dic = new Dictionary<Type, Dictionary<string, RegistObjectContext>>();

            this.customerGetMethodDic = new List<CustomerGetMethodContext>();
        }
#endif

        internal object Get(Type pType, string name, params object[] param)
        {
            CustomerGetMethodContext toRemoveContext = null;
            try
            {
                foreach (var cMethod in customerGetMethodDic)
                {
                    if (cMethod.CheckMethod(pType))
                    {
                        if (cMethod.EffectEnum == CustomerMethodEffectEnum.Once)
                        {
                            toRemoveContext = cMethod;
                        }
                        return cMethod.GetMethod(pType);
                    }
                }
                var context = GetContext(pType, name);
                var creator = context.InstCreator;
                return creator.CreateInst(context, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (toRemoveContext != null)
                {
                    customerGetMethodDic.Remove(toRemoveContext);
                }
            }
        }


        internal object Get(Type pType, params object[] param)
        {
            return this.Get(pType, DEFAULTNAME, param);
        }

        private RegistObjectContext GetContext(Type pType, string name)
        {
            try
            {
                var cDic = dic[pType];
                RegistObjectContext context;
                try
                {
                    context = cDic[name];
                }
                catch (KeyNotFoundException)
                {
                    try
                    {
                        context = cDic[DEFAULTNAME];
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new Exception(string.Format("Didn't have any inst regist as name {0}", name));
                    }
                }
                return context;
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception(string.Format("Not contains type {0}", pType.Name));
            }
        }

        private void SetContextInstType(InstType instType, RegistObjectContext context)
        {
            context.InstType = instType;
            context.InstCreator = InstCreatorFactory.Create(instType);
            var result = context.InstCreator.Check(context);
            if (result.IsPass == false)
            {
                throw new Exception("regist error:" + result.Message);
            }
        }

    }
}
