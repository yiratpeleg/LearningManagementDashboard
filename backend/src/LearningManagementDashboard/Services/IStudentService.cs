using LearningManagementDashboard.Models;

namespace LearningManagementDashboard.Services;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<Student?> GetStudentByIdAsync(Guid id);
    Task<Student> CreateStudentAsync(string fullName);
}
