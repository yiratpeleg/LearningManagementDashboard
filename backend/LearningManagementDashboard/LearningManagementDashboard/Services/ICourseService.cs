using LearningManagementDashboard.Models;

namespace LearningManagementDashboard.Services;

public interface ICourseService
{
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task<Course?> GetCourseByIdAsync(Guid id);
    Task<Course> CreateCourseAsync(string name, string description);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(Guid id);
}
