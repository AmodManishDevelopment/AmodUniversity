using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Amod.University.WebApi
{
    public static class ApiExceptionHandler
    {
        public static HttpResponseMessage HandleException(HttpActionContext context, Exception exception)
        {
            return context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An Internal Server Error has occured. please contact system administrator");
        }
    }
}
