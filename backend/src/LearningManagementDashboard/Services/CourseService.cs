using LearningManagementDashboard.Exceptions;
using LearningManagementDashboard.Models;
using System.Text;
using System.Text.Json;

namespace LearningManagementDashboard.Services;

public class CourseService : ICourseService
{
    private readonly ILogger<CourseService> _logger;
    private readonly IStorageService _storage;

    private readonly Dictionary<Guid, Course> _courses;

    private const string CoursePrefix = "courses/";
    private const string CourseSuffix = ".json";

    public CourseService(ILogger<CourseService> logger,
        IStorageService storage)
    {
        _logger = logger;
        _storage = storage;
        _courses = new();
    }

    public Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        _logger.LogInformation("Getting all courses (count: {Count})", _courses.Count);
        return Task.FromResult(_courses.Values.AsEnumerable());
    }

    public Task<Course?> GetCourseByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting course by Id: {CourseId}", id);

        if (!_courses.TryGetValue(id, out var course))
        {
            _logger.LogWarning("Course not found: {CourseId}", id);
            return Task.FromResult<Course?>(null);
        }

        return Task.FromResult<Course?>(course);
    }

    public async Task<Course> CreateCourseAsync(string name, string description)
    {
        if (CourseExistsByName(name))
        {
            _logger.LogWarning("Course name conflict: {Name}", name);
            throw new CourseAlreadyExistsException(name);
        }

        var course = CreateCourseEntity(name, description);
        _courses[course.Id] = course;

        await UploadCourseSnapshotAsync(course);

        _logger.LogInformation("Created course {CourseId} (Name: {Name})",
            course.Id, course.Name);

        return course;
    }

    public Task UpdateCourseAsync(Course course)
    {
        if (!_courses.ContainsKey(course.Id))
        {
            _logger.LogError("Failed to update: Course not found {CourseId}", course.Id);
            throw new KeyNotFoundException($"Course not found: {course.Id}");
        }

        _courses[course.Id] = course;
        _logger.LogInformation("Course {CourseId} updated", course.Id);

        return Task.CompletedTask;
    }

    public Task DeleteCourseAsync(Guid id)
    {
        if (!_courses.Remove(id))
        {
            _logger.LogError("Failed to delete: Course not found {CourseId}", id);
            throw new KeyNotFoundException($"Course not found: {id}");
        }

        _logger.LogInformation("Course {CourseId} deleted", id);

        return Task.CompletedTask;
    }

    private bool CourseExistsByName(string name)
    {
        return _courses.Values.Any(c =>
                        string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    private Course CreateCourseEntity(string name, string description) =>
    new Course
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description
    };

    private async Task UploadCourseSnapshotAsync(Course course)
    {
        var key = $"{CoursePrefix}{course.Id}{CourseSuffix}";
        var json = JsonSerializer.Serialize(course);
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        await _storage.UploadObjectAsync(key, ms);
    }
}
