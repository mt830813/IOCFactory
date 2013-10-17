using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "configSections", Namespace = "")]
    public class ConfigSections
    {
        [XmlElement(ElementName = "section")]
        public Section[] Objects { get; set; }
    }
}
