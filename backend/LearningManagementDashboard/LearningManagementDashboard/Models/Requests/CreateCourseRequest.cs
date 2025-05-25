using System.ComponentModel.DataAnnotations;

namespace LearningManagementDashboard.Models.Requests;

public class CreateCourseRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
