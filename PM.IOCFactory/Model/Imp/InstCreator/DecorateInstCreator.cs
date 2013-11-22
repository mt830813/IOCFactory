using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOCFactoryModel;
using IOCFactoryModel.Enum;
using IOCFactoryModel.Interface;
using System.Reflection;
using IOCFactory.Util;

namespace IOCFactory.Model.Imp.InstCreator
{
    public class DecorateInstCreator : IInstCreator
    {
        internal DecorateInstCreator()
        {
        }



        public RegistCheckResult Check(RegistObjectContext context)
        {
            var type = context.ObjType;
            var constructs = type.GetConstructors();
            var returnValue = new RegistCheckResult();
            returnValue.IsPass = false;
            if (constructs.Length != 1)
            {
                returnValue.Message = string.Format("type regist as decorate must and only have 1 construct method");
                return returnValue;
            }
            var contruct = constructs[0];

            var parameters = contruct.GetParameters();

            if (parameters.Length != 1)
            {
                returnValue.Message = string.Format("type regist as decorate the construct method must and only have 1 param ");
                return returnValue;
            }

            var parameter = parameters[0];

            if (parameter.ParameterType != context.PType)
            {
                returnValue.Message = string.Format("type regist as decorate the construct method's param must be {0}", context.PType.Name);
                return returnValue;
            }

            returnValue.IsPass = true;
            return returnValue;

        }


        public object CreateInst(RegistObjectContext context, params object[] param)
        {
            var list = context.Params[ContextParamNameEnum.DECORATE_CONTEXTCHAIN] as List<RegistObjectContext>;

            object returnValue = null;

            foreach (var obj in list)
            {
                if (returnValue != null)
                {
                    returnValue = obj.InstCreator.CreateInst(obj, returnValue);
                }
                else
                {
                    returnValue = obj.InstCreator.CreateInst(obj);
                }
            }

            return returnValue;
        }
    }
}
