using Amod.University.Model.Request;
using Amod.University.Model.Response;
using System;

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
        public CreateStudentResponse Create(CreateStudentRequest Student)
        {
            CreateStudentResponse result;
            string uniqueMessageId = Guid.NewGuid().ToString("N");
            try
            {
                        result = new CreateStudentResponse
                        {
                            StudentID = "NewStudentId",
                            FirstName = "None"
                        };
            }
            catch (Exception exception)
            {
                result = new CreateStudentResponse
                {
                    StudentID = "NewStudentId",
                    FirstName = "None"

                };
            }

            return result;
        }
    }
}
