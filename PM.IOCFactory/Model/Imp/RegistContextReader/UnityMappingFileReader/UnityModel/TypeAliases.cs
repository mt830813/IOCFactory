using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "typeAliases", Namespace = "")]
    public class TypeAliases
    {
        [XmlElement(ElementName = "typeAlias")]
        public TypeAlias[] Objects { get; set; }
    }
}
