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
    /// The CreateStudentResponse class.
    /// </summary>
    [Serializable]
    [DataContract(Name = "CreateStudentResponse", Namespace = "Amod.University.Model.Response")]
    [XmlRoot(ElementName = "CreateStudentResponse", Namespace = "Amod.University.Model.Response")]
    public class CreateStudentResponse : BaseResponse
    {
        /// <summary>
        /// ID for new Student 
        /// </summary>
        [DataMember(Name = "StudentID")]
        [XmlElement("StudentID")]
        public string StudentID { get; set; }

        /// <summary>
        /// Student First Name
        /// </summary>
        [DataMember(Name = "FirstName")]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Student Last Name
        /// </summary>
        [DataMember(Name = "LastName")]
        [XmlElement("LastName")]
        public string LastName { get; set; }

    }
}
