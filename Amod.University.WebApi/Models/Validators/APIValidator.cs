using Amod.University.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Amod.University.WebApi.Models.Validators
{
    public static class APIValidator
    {
        public static bool ValidateCreateStudentRequest(this CreateStudentRequest request, out string message)
        {
            var errorResponseMessage = new StringBuilder();
            bool result = false;
            if (null == request)
            {
                errorResponseMessage.Append("Unable to parse the request. please review the input data and resend.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Student.FirstName))
                {
                    errorResponseMessage.AppendFormat("Field: FirstName, Issues: {0}",
                      "Student First Name needs to be provided.").AppendLine();
                }

                if (string.IsNullOrWhiteSpace(request.Student.LastName))
                {
                    errorResponseMessage.AppendFormat("Field: LastName, Issues: {0}",
                      "Student Last Name needs to be provided.").AppendLine();
                }
            }

            message = errorResponseMessage.ToString();
            if (string.IsNullOrEmpty(message))
            {
                result = true;
            }
            return result;
        }
    }
}