using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Amod.University.WebApi.Managers.Student;
using Amod.University.WebApi.Models.Validators;
using Amod.University.ServiceLocator;
using Amod.University.Model.Request;
using Amod.University.Model.Response;

namespace Amod.University.WebApi.Controllers
{
    public class StudentController : ApiController
    {
        /// <summary>
        /// The Student manager
        /// </summary>
        IStudentManager _StudentManager;

        /// <summary>
        /// Ping University Service
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("IsAlive")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, string.Format("Welcome to Amod Web API2 University: CurrentTime: {0}", DateTime.UtcNow));
        }

        /// <summary>
        /// Create a New Student
        /// </summary>
        /// <param name="request">Create Student Request</param>
        [HttpPost]
        [Route("CreateStudent")]
        public HttpResponseMessage CreateStudent(CreateStudentRequest request)
        {
            //Give another chance to re-read from the Pay load
            if (null == request)
                request = Request.Content.ReadAsAsync<CreateStudentRequest>().Result;
            _StudentManager = UniversityServiceLocator.Instance.GetInstance<IStudentManager>();
            if (_StudentManager is StudentManager)
            {
                string validationMessage;
                if (!request.ValidateCreateStudentRequest(out validationMessage))
                {
                    var response = CreateBadRequestResponse<CreateStudentResponse>(validationMessage);
                    return response;
                }
            }

            return Request.CreateResponse(HttpStatusCode.Created, _StudentManager.CreateStudent(request));
        }

        /// <summary>
        /// Gets all Students
        /// </summary>
        [HttpGet]
        [Route("GetStudents")]
        public HttpResponseMessage GetStudents()
        {
            _StudentManager = UniversityServiceLocator.Instance.GetInstance<IStudentManager>();
            return Request.CreateResponse(HttpStatusCode.OK, _StudentManager.GetStudents());
        }

        private HttpResponseMessage CreateBadRequestResponse<T>(string validationMessage) where T : BaseResponse, new()
        {
            T result = new T
            {
                ResultMessage = validationMessage
            };

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, result);
            return response;
        }
    }
}