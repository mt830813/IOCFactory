using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOCFactoryModel.Interface;
using System.Runtime.Serialization;
using IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel;
using System.IO;
using IOCFactoryModel;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader
{
    internal class Reader : IRegistContextReader
    {
        private Dictionary<LifeTimeEnum, InstType> mapping;

        public Reader()
        {
            mapping = new Dictionary<LifeTimeEnum, InstType>();

            mapping.Add(LifeTimeEnum.transient, InstType.DI);

            mapping.Add(LifeTimeEnum.singleton, InstType.DISingleton);
        }

        public IOCFactoryModel.RegistMappingContextCollection GetMappingContexts(string fileUrl)
        {
            try
            {
                var ser = new XmlSerializer(typeof(Configuration));
                var returnValue = new RegistMappingContextCollection();
                using (var fs = File.Open(fileUrl, FileMode.Open))
                {
                    var config = (Configuration)ser.Deserialize(fs);
                    var alias = config.Unity.TypeAliases.Objects;

                    var types = new List<UnityType>();

                    foreach (var container in config.Unity.Containers.Objects)
                    {
                        types.AddRange(container.Types.Objects);
                    }



                    foreach (var type in types)
                    {
                        var newObj = new RegistMappingContext();

                        newObj.PTypeStr = GetFromAlias(alias, type.TypeStr);

                        string cType = type.MapTo;
                        if (string.IsNullOrWhiteSpace(cType))
                        {
                            newObj.CTypeStr = newObj.PTypeStr;
                        }
                        else
                        {
                            newObj.CTypeStr = GetFromAlias(alias, cType);
                        }

                        newObj.InstTypeStr = mapping[type.LifeTime.Type].ToString();

                        returnValue.Contexts.Add(newObj);
                    }
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetFromAlias(IEnumerable<TypeAlias> list, string name)
        {
            var obj = list.FirstOrDefault(p => p.Alias == name);

            if (obj != null)
            {
                return obj.TypeStr;
            }
            else
            {
                return name;
            }
        }
    }
}
