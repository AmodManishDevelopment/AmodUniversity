using Amod.University.Model.Request;
using Amod.University.Model.Response;
using System;
using System.Collections.Generic;
using Amod.University.Model.Entities;

namespace Amod.University.WebApi.Managers.Student
{
    /// <summary>
    /// The StudentManager class.
    /// </summary>
    public class StudentManager : IStudentManager
    {
        /// <summary>
        /// Creates the specified Student.
        /// </summary>
        /// <param name="Student">The Student.</param>
        /// <returns>The result of the create Student action.</returns>
        public CreateStudentResponse CreateStudent(CreateStudentRequest request)
        {
            CreateStudentResponse result;
            string uniqueMessageId = Guid.NewGuid().ToString("N");
            try
            {
                        result = new CreateStudentResponse
                        {
                            StudentID = 1,
                            FirstName = request.Student.FirstName,
                            LastName = request.Student.LastName,
                            ResultMessage = request.Student.FirstName + " " + request.Student.LastName + " Enrolled Successfully!"
                        };
            }
            catch (Exception ex)
            {
                result = new CreateStudentResponse
                {
                    ResultMessage = ex.Message
                };
            }

            return result;
        }

        /// <summary>
        /// Gets all Students.
        /// </summary>
        /// <returns>The result of the create Student action.</returns>
        public GetStudentsResponse GetStudents()
        {
            GetStudentsResponse result;

            try
            {
                List<Model.Entities.Student> students = new List<Model.Entities.Student>();
                for (int i = 1; i <= 5; i++)
                {
                    students.Add(new Model.Entities.Student() { StudentID = i, FirstName = "FirstName" + i.ToString(), LastName = "LastName" + i.ToString() });
                }
                     
                result = new GetStudentsResponse
                {
                    Students = students,
                    ResultMessage = students.Count + " students returned."
                };
            }
            catch (Exception ex)
            {
                result = new GetStudentsResponse
                {
                    ResultMessage = ex.Message
                };
            }

            return result;
        }
    }
}
