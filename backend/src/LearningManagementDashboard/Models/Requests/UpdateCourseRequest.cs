using System.ComponentModel.DataAnnotations;

namespace LearningManagementDashboard.Models.Requests;

public class UpdateCourseRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = default!;

    [StringLength(500)]
    public string? Description { get; set; }
}
