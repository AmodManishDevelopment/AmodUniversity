using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amod.University.Data.Helper
{
    [Serializable]
    public class DataClientResult
    {
        public Boolean IsFunctionalError { get; set; }

        public Boolean IsOperationalError { get; set; }

        public Boolean IsSuccess { get; set; }

        public Exception ErrorException { get; set; }

        public String Message { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DataClientResult"/> class.
        /// </summary>
        public DataClientResult()
        {
            IsSuccess = true;
            ErrorException = null;
            Message = "";
            IsOperationalError = false;
            IsFunctionalError = false;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override String ToString()
        {
            StringBuilder toStringSB = new StringBuilder(1000);
            toStringSB.AppendFormat("Success: {0} Functional Error: {1} Operational Error: {2} Exception: {3} Message: {4}",
                IsSuccess, IsFunctionalError, IsOperationalError, null != ErrorException ? ErrorException.ToString() : "", Message);
            return (toStringSB.ToString());
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DataClientResult"/> class.
        /// </summary>
        /// <param name="exp">The exp.</param>
        public DataClientResult(Exception exp)
        {
            if (null != exp)
            {
                IsFunctionalError = false;
                IsOperationalError = true;
                IsSuccess = false;
                ErrorException = exp;
                Message = exp.Message;
            }
        }
    }
}
