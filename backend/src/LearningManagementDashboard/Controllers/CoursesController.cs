using AutoMapper;
using LearningManagementDashboard.Models.Requests;
using LearningManagementDashboard.Models.Responses;
using LearningManagementDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IEnrolmentService _enrolmentService;
    private readonly IMapper _mapper;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService,
        IEnrolmentService enrolmentService,
        IMapper mapper,
        ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        _enrolmentService = enrolmentService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetAllCourses")]
    public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
    {
        _logger.LogInformation("GET /api/courses called");

        var courses = await _courseService.GetAllCoursesAsync();
        var coursesResponse = _mapper.Map<IEnumerable<CourseResponse>>(courses);

        return Ok(coursesResponse);
    }

    [HttpGet("{id}", Name = "GetCourseById")]
    public async Task<ActionResult<CourseResponse>> GetById(Guid id)
    {
        _logger.LogInformation("GET /api/courses/{CourseId} called", id);

        var course = await _courseService.GetCourseByIdAsync(id);
        if (course is null)
            return NotFound();

        var courseResponse = _mapper.Map<CourseResponse>(course);
        return Ok(courseResponse);
    }

    [HttpPost(Name = "CreateCourse")]
    public async Task<ActionResult<CourseResponse>> Create(
        [FromBody] CreateCourseRequest req)
    {
        _logger.LogInformation("POST /api/courses (Name: {Name})", req.Name);

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var createdCourse = await _courseService.CreateCourseAsync(req.Name, req.Description);
        _logger.LogInformation("Course created: {CourseId}", createdCourse.Id);

        var courseResponse = _mapper.Map<CourseResponse>(createdCourse);

        return CreatedAtAction(
            nameof(GetById),
            new { id = courseResponse.Id },
            courseResponse
        );
    }

    [HttpPut("{id}", Name = "UpdateCourse")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCourseRequest req)
    {
        _logger.LogInformation("PUT /api/courses/{CourseId} called", id);

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var existing = await _courseService.GetCourseByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Name = req.Name;
        existing.Description = req.Description;
        await _courseService.UpdateCourseAsync(existing);

        _logger.LogInformation("Course {CourseId} updated", id);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "DeleteCourse")]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("DELETE /api/courses/{CourseId} called", id);

        await _courseService.DeleteCourseAsync(id);
        await _enrolmentService.DeleteEnrolmentByCourseIdAsync(id);

        _logger.LogInformation("Course {CourseId} deleted", id);
        return NoContent();
    }
}
