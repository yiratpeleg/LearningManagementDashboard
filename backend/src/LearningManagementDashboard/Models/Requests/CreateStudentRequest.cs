using System.ComponentModel.DataAnnotations;

namespace LearningManagementDashboard.Models.Requests;

public class CreateStudentRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = default!;
}
