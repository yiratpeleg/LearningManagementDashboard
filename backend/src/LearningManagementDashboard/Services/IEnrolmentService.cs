using LearningManagementDashboard.Models;

namespace LearningManagementDashboard.Services;

public interface IEnrolmentService
{
    Task<IEnumerable<Enrolment>> GetAllEnrolmentsAsync();
    Task<Enrolment?> GetEnrolmentByIdAsync(Guid id);
    Task<IEnumerable<Enrolment>> GetEnrolmentsByStudentAsync(Guid studentId);
    Task<Enrolment> CreateEnrolmentAsync(Guid courseId, Guid studentId);
    Task<IEnumerable<EnrolmentReportItem>> GenerateEnrolmentReportAsync();
}
