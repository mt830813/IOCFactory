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
        private volatile Dictionary<LifeTimeEnum, InstType> mapping;

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
                var returnValue = new RegistMappingContextCollection();
                using (var fs = File.Open(fileUrl, FileMode.Open))
                {
                    returnValue = this.GetMappingContexts(fs);
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RegistMappingContextCollection GetMappingContexts(Stream stream)
        {
            var returnValue = new RegistMappingContextCollection();
            try
            {
                var ser = new XmlSerializer(typeof(Unity));
                var config = (Unity)ser.Deserialize(stream);



                var types = new List<UnityType>();

                foreach (var container in config.Containers)
                {

                    types.AddRange(container.Types);
                }

                foreach (var type in types)
                {
                    var newObj = new RegistMappingContext();

                    newObj.PTypeStr = type.TypeStr;

                    string cType = type.MapTo;
                    if (string.IsNullOrWhiteSpace(cType))
                    {
                        newObj.CTypeStr = newObj.PTypeStr;
                    }
                    else
                    {
                        newObj.CTypeStr = cType;
                    }

                    newObj.InstTypeStr = mapping[type.LifeTime.Type].ToString();

                    newObj.Name = type.Name;

                    returnValue.Contexts.Add(newObj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

    }
}
