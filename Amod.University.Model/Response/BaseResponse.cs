using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Amod.University.Model.Response
{
    [Serializable]
    [DataContract(Name = "BaseResponse", Namespace = "Amod.University.Model.Response")]
    [XmlRoot(ElementName = "BaseResponse", Namespace = "Amod.University.Model.Response")]
    public class BaseResponse
    {
        [DataMember(Order = 1, IsRequired = true, Name = "resultMessage")]
        [XmlElement("resultMessage")]
        public string ResultMessage { get; set; }
    }
}
