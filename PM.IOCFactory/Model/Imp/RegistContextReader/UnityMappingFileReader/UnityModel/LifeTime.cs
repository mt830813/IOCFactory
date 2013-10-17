using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IOCFactory.Model.Imp.RegistContextReader.UnityMappingFileReader.UnityModel
{
    [XmlRoot(ElementName = "lifetime", Namespace = "")]
    public class LifeTime
    {
        [XmlAttribute(AttributeName = "type")]
        public LifeTimeEnum Type { get; set; }

        public LifeTime()
        {
            this.Type = LifeTimeEnum.transient;
        }
    }
}
