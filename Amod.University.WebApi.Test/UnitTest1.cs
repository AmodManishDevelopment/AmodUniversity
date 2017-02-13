using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amod.University.Model.Request;
using Amod.RestChannelPlatformProvider;
using Amod.University.Model.Response;

namespace Amod.University.WebApi.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateStudentTest()
        {
            CreateStudentRequest request = new CreateStudentRequest();
            request.Student = new Model.Entities.Student() { FirstName = "Manish", LastName = "Narayan" };

            RestPlatformChannelProvider Instance = new RestPlatformChannelProvider();
            var response = Instance.Post<CreateStudentResponse>(RestSharp.DataFormat.Json, "http://localhost:33472/createstudent", request);
            Assert.IsNotNull(response.Data);
            Assert.IsTrue(response.Data.StudentID > 0);
        }
    }
}
