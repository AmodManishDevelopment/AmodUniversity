using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Amod.University.Model.Request
{
    /// <summary>
    /// Student Registeration Request class
    /// </summary>
    [Serializable]
    [DataContract(Name = "StudentRegistrationRequest", Namespace = "")]
    public class CreateStudentRequest
    {
        /// <summary>
        /// Student First Name.
        /// </summary>
        [DataMember(Order = 1, Name = "FirstName", EmitDefaultValue = true)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Student Last Name.
        /// </summary>
        [DataMember(Order = 2, Name = "LastName", EmitDefaultValue = true)]
        [XmlElement("LastName")]
        public string LastName { get; set; }
    }
}
