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
using System.Configuration;

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

        private volatile Dictionary<Type, Dictionary<string, RegistObjectContext>> dic;

        private delegate T customerGetMethodDelegate<T>();

        private volatile List<CustomerGetMethodContext> customerGetMethodDic;

        private static Factory factory;

        private object _locker = new object();
        private InstType[] NotAllowNormalRegistType = { InstType.Decorate, InstType.ObjectPool };

        private InstType[] NotAllowDecorateRegistType = { InstType.Decorate, InstType.DI, InstType.DISingleton };

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

                this.RegistUseMappingContext(contexts);

            }
            else
            {
                throw new FileNotFoundException("fileNotFound", filePath);
            }
        }

        public void RegistFromSection(string sectionName)
        {
            try
            {
                var obj = ConfigurationManager.GetSection(sectionName) as RegistMappingContextCollection;
                this.RegistUseMappingContext(obj);
            }
            catch (Exception ex)
            {
                throw ex;
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
            if (NotAllowNormalRegistType.Contains(instType))
            {
                throw new Exception(string.Format("{0} Inst Not Allow Use Normal Regist Method To Regist", Enum.GetName(typeof(InstType), instType)));
            }
            this.RegistContext(pType, cType, instType, name);
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
            lock (_locker)
            {
                customerGetMethodDic.Add(
                    new CustomerGetMethodContext()
                    {
                        CheckMethod = checkMethod,
                        GetMethod = getMethod,
                        EffectEnum = expectEffectRange
                    });
            }
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
        public void RegistDecorate<T, Q>(InstType instType) where Q : T
        {
            RegistDecorate<T, Q>(DEFAULTNAME, instType);
        }


        /// <summary>
        /// 以装饰者模式进行注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Q"></typeparam>
        /// <param name="name">要装饰的已注册的对象</param>
        /// <param name="instType">对象生成类型</param>
        public void RegistDecorate<T, Q>(string name, InstType instType) where Q : T
        {
            RegistObjectContext context;
            RegistObjectContext dContext;

            if (NotAllowDecorateRegistType.Contains(instType))
            {
                throw new Exception(string.Format("{0} Inst Not Allow Use Decorate Regist Method To Regist", Enum.GetName(typeof(InstType), instType)));
            }

            try
            {
                dContext = GetContext(typeof(T), name, false);

                if (dContext.InstType != InstType.Decorate)
                {
                    var tContext = CreateContext(typeof(T), typeof(Q), InstType.Decorate);
                    tContext.Params.Add(ContextParamNameEnum.DECORATE_CONTEXTCHAIN, new List<RegistObjectContext>() { dContext });
                    SetContext(typeof(T), tContext, name, true);
                    dContext = tContext;
                }
            }
            catch (KeyNotFoundException)
            {
                Regist<T, Q>(name, instType);
                return;
            }
            context = CreateContext(typeof(T), typeof(Q), instType);

            var list = dContext.Params[ContextParamNameEnum.DECORATE_CONTEXTCHAIN] as List<RegistObjectContext>;

            list.Add(context);
        }

        public void RegistObjectPool<T, Q>(int maxPoolCount, Action<Q> action) where Q : T
        {
            RegistObjectPool<T, Q>(DEFAULTNAME, maxPoolCount, action);
        }

        public void RegistObjectPool<T, Q>(string name, int maxPoolCount, Action<Q> action) where Q : T
        {
            var context = RegistContext(typeof(T), typeof(Q), InstType.ObjectPool, name);

            context.Params.Add(ContextParamNameEnum.POOL_MAXCOUNT, maxPoolCount);

            Action<object> tempAc = new Action<object>(o => { });
            if (action != null)
            {
                tempAc = new Action<object>(o => action((Q)o));
            }

            context.Params.Add(ContextParamNameEnum.POOL_INITACTION, tempAc);
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
#if DEBUG
            return (T)this.Get(typeof(T), name, param);
#else
            try
            {
                return (T)this.Get(typeof(T), name, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
#endif

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
            lock (this._locker)
            {
                this.dic = new Dictionary<Type, Dictionary<string, RegistObjectContext>>();

                this.customerGetMethodDic = new List<CustomerGetMethodContext>();
            }
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
#if DEBUG
#else
            catch (Exception ex)
            {
                throw ex;
            }
#endif
            finally
            {
                if (toRemoveContext != null)
                {
                    customerGetMethodDic.Remove(toRemoveContext);
                }
            }
        }

        private RegistObjectContext CreateContext(Type pType, Type cType, InstType instType)
        {
            RegistObjectContext context = new RegistObjectContext();

            context.ObjType = cType;
            context.PType = pType;

            this.SetContextInstType(instType, context);
            return context;
        }

        private RegistObjectContext RegistContext(Type pType, Type cType, InstType instType, string name)
        {
            var context = this.CreateContext(pType, cType, instType);
            SetContext(pType, context, name, false);
            return context;
        }

        internal object Get(Type pType, params object[] param)
        {
            return this.Get(pType, DEFAULTNAME, param);
        }

        private void RegistUseMappingContext(RegistMappingContextCollection collection)
        {
            foreach (var context in collection.Contexts)
            {
                this.Regist(context.PType, context.CType, context.InstType, context.Name);
            }
        }

        private RegistObjectContext GetContext(Type pType, string name, bool isUseDefaultName = true)
        {
            Dictionary<string, RegistObjectContext> cDic;
            try
            {
                cDic = dic[pType];
            }
            catch (KeyNotFoundException)
            {
                throw new Exception(string.Format("Not contains type {0}", pType.Name));
            }
            RegistObjectContext context;
            try
            {
                context = cDic[name];
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    if (isUseDefaultName)
                    {
                        context = cDic[DEFAULTNAME];
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException(string.Format("Didn't have any inst regist as name {0}", name));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return context;
        }

        private void SetContext(Type pType, RegistObjectContext context, string name, bool allowOverWrite)
        {
            try
            {
                lock (_locker)
                {
                    Dictionary<string, RegistObjectContext> cDic = null;
                    try
                    {
                        cDic = dic[pType];
                    }
                    catch (KeyNotFoundException)
                    {

                        dic.Add(pType, new Dictionary<string, RegistObjectContext>());
                        cDic = dic[pType];
                    }

                    if (string.IsNullOrEmpty(name))
                    {
                        name = DEFAULTNAME;
                    }

                    try
                    {
                        cDic.Add(name, context);
                    }
                    catch (ArgumentException)
                    {
                        if (allowOverWrite)
                        {
                            cDic[name] = context;
                        }
                        else
                        {
                            throw new Exception(string.Format("Regist Type '{0}' Must be Unique", name));
                        }

                    }
                }

            }
            catch (KeyNotFoundException)
            {
                throw new Exception(string.Format("Not contains type {0}", pType.Name));
            }
        }

        private void SetContextInstType(InstType instType, RegistObjectContext context, bool isCheck = true)
        {
            context.InstType = instType;
            context.InstCreator = InstCreatorFactory.Create(instType);
            if (isCheck)
            {
                var result = context.InstCreator.Check(context);
                if (result.IsPass == false)
                {
                    throw new Exception("regist error:" + result.Message);
                }
            }
        }
    }
}
