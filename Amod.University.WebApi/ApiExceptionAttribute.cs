using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Amod.University.WebApi
{
    public class ApiExceptionAttribute : Attribute, IExceptionFilter
    {
        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            try
            {
                actionExecutedContext.Response = ApiExceptionHandler.HandleException(actionExecutedContext.ActionContext, actionExecutedContext.Exception);
            }
            catch
            {
                actionExecutedContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
            return Task.FromResult<object>(null);
        }

        public bool AllowMultiple
        {
            get { return false; }
        }
    }
}
