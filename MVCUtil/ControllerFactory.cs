using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IOCFactory;

namespace MVCUtil
{
    public class ControllerFactory : DefaultControllerFactory
    {
        private DefaultControllerFactory defaultFactory;

        private Factory iocFactory;

        public ControllerFactory()
        {
            defaultFactory = new DefaultControllerFactory();

            iocFactory = Factory.GetInst();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                if (!iocFactory.IsRegist<IController>(controllerType.FullName))
                {
                    iocFactory.Regist<IController>(controllerType, IOCFactoryModel.InstType.DI, controllerType.FullName);
                }
                return iocFactory.Get<IController>(controllerType.FullName);
            }
            else
            {
                return null;
            }

        }
    }
}
