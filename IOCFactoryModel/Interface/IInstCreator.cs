using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOCFactoryModel.Interface
{
    public interface IInstCreator
    {
        //T CreateInst<T>(RegistContext<T> context);

        RegistCheckResult Check(RegistObjectContext context);

        object CreateInst(RegistObjectContext context, params object[] param);
    }
}
