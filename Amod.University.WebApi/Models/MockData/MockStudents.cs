using Amod.University.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amod.University.WebApi.Models.MockData
{
    public static class MockStudents
    {
        public static List<Student> GetMockStudents(int size)
        {
            int StudentID = 1;
            return Enumerable.Range(0, size).Select(o => new Student { StudentID = StudentID++, FirstName = "FirstName" + StudentID.ToString(), LastName = "LastName" + StudentID.ToString() }).ToList();
        }
    }
}