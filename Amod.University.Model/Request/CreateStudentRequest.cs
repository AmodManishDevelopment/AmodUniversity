using Amod.University.Model.Entities;
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
    [DataContract(Name = "CreateStudentRequest", Namespace = "")]
    public class CreateStudentRequest
    {
        /// <summary>
        /// Student
        /// </summary>
        [DataMember(Order = 1, Name = "Student", EmitDefaultValue = true)]
        [XmlElement("Student")]
        public Student Student { get; set; }

        /// <summary>
        /// Student
        /// </summary>
        [DataMember(Order = 2, Name = "EnrollmentNotes", EmitDefaultValue = true)]
        [XmlElement("EnrollmentNotes")]
        public string EnrollmentNotes { get; set; }
    }
}
