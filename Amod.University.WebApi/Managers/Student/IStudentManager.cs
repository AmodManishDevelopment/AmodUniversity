using Amod.University.Model.Request;
using Amod.University.Model.Response;

namespace Amod.University.WebApi.Managers.Student
{
    /// <summary>
    /// The IStudentManager interface.
    /// </summary>
    public interface IStudentManager
    {
        /// <summary>
        /// Creates the specified Student.
        /// </summary>
        /// <param name="Student">The Student.</param>
        /// <returns>
        /// The result of the create Student action.
        /// </returns>
        CreateStudentResponse Create(CreateStudentRequest Student);
    }
}
