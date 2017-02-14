using Amod.University.Model.Request;
using Amod.University.Model.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Amod.University.Model.Entities;
using Amod.University.WebApi.Models.MockData;

namespace Amod.University.WebApi.Managers.Student
{
    /// <summary>
    /// The Mock StudentManager class.
    /// </summary>
    public class MockStudentManager : IStudentManager
    {
        List<Model.Entities.Student> students = MockStudents.GetMockStudents(5);
        /// <summary>
        /// Creates the specified Student.
        /// </summary>
        /// <param name="Student">The Student.</param>
        /// <returns>The result of the create Student action.</returns>
        public CreateStudentResponse CreateStudent(CreateStudentRequest request)
        {
            CreateStudentResponse createStudentResponse = null;
            try
            {
                students.Add(new Model.Entities.Student() { StudentID = students.Count + 1, FirstName = request.Student.FirstName, LastName = request.Student.LastName });

                createStudentResponse = new CreateStudentResponse() {
                    StudentID = students.Count + 1, 
                    FirstName = request.Student.FirstName, 
                    LastName = request.Student.LastName, 
                    ResultMessage = request.Student.FirstName + " " + request.Student.LastName + " Enrolled Successfully!" };
            }
            catch (Exception ex)
            {
                createStudentResponse = new CreateStudentResponse
                {
                    ResultMessage = ex.Message
                };
            }

            return createStudentResponse;
        }

        /// <summary>
        /// Gets all students.
        /// </summary>
        /// <returns>The result of the Get Students action.</returns>
        public GetStudentsResponse GetStudents()
        {
            GetStudentsResponse getStudentsResponse = null;
            try
            {
                getStudentsResponse = new GetStudentsResponse()
                {
                    Students = students,
                    ResultMessage = students.Count + " students returned."
                };
            }
            catch (Exception ex)
            {
                getStudentsResponse = new GetStudentsResponse
                {
                    ResultMessage = ex.Message
                };
            }

            return getStudentsResponse;
        }
    }
}
