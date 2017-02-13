using Amod.University.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Amod.University.Model.Response
{
    /// <summary>
    /// The GetStudentsResponse class.
    /// </summary>
    [Serializable]
    [DataContract(Name = "GetStudentsResponse", Namespace = "Amod.University.Model.Response")]
    [XmlRoot(ElementName = "GetStudentsResponse", Namespace = "Amod.University.Model.Response")]
    public class GetStudentsResponse : BaseResponse
    {
        /// <summary>
        /// ID for new Student 
        /// </summary>
        [DataMember(Name = "students")]
        [XmlElement("students")]
        public List<Student> Students { get; set; }
    }
}
