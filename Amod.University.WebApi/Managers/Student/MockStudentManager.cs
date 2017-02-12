using Amod.University.Model.Request;
using Amod.University.Model.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Amod.University.WebApi.Managers.Student
{
    /// <summary>
    /// The Mock StudentManager class.
    /// </summary>
    public class MockStudentManager : IStudentManager
    {
        /// <summary>
        /// Creates the specified Student.
        /// </summary>
        /// <param name="Student">The Student.</param>
        /// <returns>The result of the create Student action.</returns>
        public CreateStudentResponse Create(CreateStudentRequest request)
        {
            CreateStudentResponse createStudentResponse = null;
            try
            {
                createStudentResponse = new CreateStudentResponse() { StudentID = "1", FirstName = request.FirstName, LastName = request.LastName, ResultMessage = request.FirstName + " " + request.LastName + " Enrolled Successfully!" };
            }
            catch (Exception e)
            {
                throw new Exception(" Error occurred while processing Create Student changeAlertRequest- " + e.Message.ToString());
            }

            return createStudentResponse;

        }

    }
}
