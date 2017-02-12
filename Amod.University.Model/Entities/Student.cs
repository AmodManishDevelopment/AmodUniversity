using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Amod.University.Model.Entities
{
    /// <summary>
    /// Student class
    /// </summary>
    [Serializable]
    [DataContract(Name = "Student", Namespace = "Amod.University.Model.Entities")]
    public class Student
    {
        /// <summary>
        /// Student ID.
        /// </summary>
        [DataMember(Order = 1, Name = "StudentID", EmitDefaultValue = true)]
        [XmlElement("StudentID")]
        public int StudentID { get; set; }

        /// <summary>
        /// Student First Name.
        /// </summary>
        [DataMember(Order = 2, Name = "FirstName", EmitDefaultValue = true)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Student First Name.
        /// </summary>
        [DataMember(Order = 3, Name = "LastName", EmitDefaultValue = true)]
        [XmlElement("LastName")]
        public string LastName { get; set; }
    }
}
