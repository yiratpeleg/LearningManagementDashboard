namespace LearningManagementDashboard.Models.Responses;

public class EnrolmentResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime EnrolledAt { get; set; }
}
