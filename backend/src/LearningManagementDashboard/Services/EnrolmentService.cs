using LearningManagementDashboard.Exceptions;
using LearningManagementDashboard.Models;

namespace LearningManagementDashboard.Services;

public class EnrolmentService : IEnrolmentService
{
    private readonly ILogger<EnrolmentService> _logger;
    private readonly ICourseService _courseService;
    private readonly IStudentService _studentService;
    private readonly Dictionary<Guid, Enrolment> _enrolments;

    public EnrolmentService(
        ILogger<EnrolmentService> logger,
        ICourseService courseService,
        IStudentService studentService)
    {
        _logger = logger;
        _courseService = courseService;
        _studentService = studentService;
        _enrolments = new();
    }

    public Task<IEnumerable<Enrolment>> GetAllEnrolmentsAsync()
    {
        _logger.LogInformation("Getting all enrolments (count {Count})", _enrolments.Count);
        return Task.FromResult(_enrolments.Values.AsEnumerable());
    }

    public Task<Enrolment?> GetEnrolmentByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting enrolment by Id {EnrolmentId}", id);

        if (!_enrolments.TryGetValue(id, out var enrolment))
        {
            _logger.LogWarning("Enrolment not found: {EnrolmentId}", id);
            return Task.FromResult<Enrolment?>(null);
        }

        return Task.FromResult<Enrolment?>(enrolment);
    }

    public Task<IEnumerable<Enrolment>> GetEnrolmentsByStudentAsync(Guid studentId)
    {
        _logger.LogInformation("Getting enrolments for Student {StudentId}", studentId);

        var enrolments = _enrolments.Values
            .Where(e => e.StudentId == studentId)
            .AsEnumerable();

        return Task.FromResult(enrolments);
    }

    public async Task<Enrolment> CreateEnrolmentAsync(Guid courseId, Guid studentId)
    {
        await EnsureCourseExistsAsync(courseId);
        await EnsureStudentExistsAsync(studentId);
        EnsureNoConflict(courseId, studentId);

        var enrolment = CreateEnrolmentEntity(courseId, studentId);
        _enrolments[enrolment.Id] = enrolment;
        _logger.LogInformation(
            "Created enrolment {EnrolId} (Course {CourseId}, Student {StudentId})",
            enrolment.Id, courseId, studentId);

        return enrolment;
    }

    public Task<IEnumerable<EnrolmentReportItem>> GenerateEnrolmentReportAsync()
    {
        _logger.LogInformation("Generating enrolment report");
        var report = _enrolments.Values
            .GroupBy(e => e.CourseId)
            .Select(g => new EnrolmentReportItem
            {
                CourseId = g.Key,
                StudentCount = g.Count()
            });
        return Task.FromResult(report);
    }

    private async Task EnsureCourseExistsAsync(Guid courseId)
    {
        if (await _courseService.GetCourseByIdAsync(courseId) is null)
        {
            _logger.LogError("Course not found {CourseId}", courseId);
            throw new KeyNotFoundException($"Course not found: {courseId}");
        }
    }

    private async Task EnsureStudentExistsAsync(Guid studentId)
    {
        if (await _studentService.GetStudentByIdAsync(studentId) is null)
        {
            _logger.LogError("Student not found {StudentId}", studentId);
            throw new KeyNotFoundException($"Student not found: {studentId}");
        }
    }

    private void EnsureNoConflict(Guid courseId, Guid studentId)
    {
        if (_enrolments.Values.Any(e =>
                e.CourseId == courseId &&
                e.StudentId == studentId))
        {
            _logger.LogWarning(
                "Enrolment already exists for Course {CourseId}, Student {StudentId}",
                courseId, studentId);
            throw new EnrolmentAlreadyExistsException(courseId, studentId);
        }
    }

    private Enrolment CreateEnrolmentEntity(Guid courseId, Guid studentId) =>
        new Enrolment
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            StudentId = studentId,
            EnrolledAt = DateTime.UtcNow
        };
}
