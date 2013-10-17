using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IOCFactoryModel.Interface
{
    public interface IRegistContextReader
    {
        RegistMappingContextCollection GetMappingContexts(string fileUrl);
    }
}
