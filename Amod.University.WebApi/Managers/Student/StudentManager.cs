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
    }
}
