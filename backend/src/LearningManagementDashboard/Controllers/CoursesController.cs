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
    private readonly IMapper _mapper;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService,
        IMapper mapper,
        ILogger<CoursesController> logger)
    {
        _courseService = courseService;
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
}
