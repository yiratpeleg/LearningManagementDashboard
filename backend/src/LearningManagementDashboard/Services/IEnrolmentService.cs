using LearningManagementDashboard.Models;

namespace LearningManagementDashboard.Services;

public interface IEnrolmentService
{
    Task<IEnumerable<Enrolment>> GetAllEnrolmentsAsync();
    Task<Enrolment> CreateEnrolmentAsync(Guid courseId, Guid studentId);
    Task<IEnumerable<EnrolmentReportItem>> GenerateEnrolmentReportAsync();
    Task DeleteEnrolmentByCourseIdAsync(Guid courseId);
}
