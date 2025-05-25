using System.ComponentModel.DataAnnotations;

namespace LearningManagementDashboard.Models.Requests;

public class CreateEnrolmentRequest
{
    [Required]
    public Guid CourseId { get; set; }

    [Required]
    public Guid StudentId { get; set; }
}
