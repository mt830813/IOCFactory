using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "configuration", Namespace = "")]
    public class Configuration
    {
        [XmlElement(ElementName = "configSections")]
        public ConfigSections ConfigSections { get; set; }

        [XmlElement(ElementName = "unity")]
        public Unity Unity { get; set; }
    }
}
