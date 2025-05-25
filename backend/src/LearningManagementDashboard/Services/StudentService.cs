using LearningManagementDashboard.Models;

namespace LearningManagementDashboard.Services;

public class StudentService : IStudentService
{
    private readonly ILogger<StudentService> _logger;
    private readonly Dictionary<Guid, Student> _students;

    public StudentService(ILogger<StudentService> logger)
    {
        _logger = logger;
        _students = new();
    }

    public Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        _logger.LogInformation("Getting all students (count: {Count})", _students.Count);
        return Task.FromResult(_students.Values.AsEnumerable());
    }

    public Task<Student?> GetStudentByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting student by Id: {StudentId}", id);
        if (!_students.TryGetValue(id, out var student))
        {
            _logger.LogWarning("Student not found: {StudentId}", id);
            return Task.FromResult<Student?>(null);
        }
        return Task.FromResult(student);
    }

    public Task<Student> CreateStudentAsync(string fullName)
    {
        Student student = CreateStudent(fullName);
        _students[student.Id] = student;

        _logger.LogInformation("Created student {StudentId} (Name: {Name})",
                               student.Id, student.FullName);

        return Task.FromResult(student);
    }

    private Student CreateStudent(string fullName)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            FullName = fullName
        };
    }
}
