using LearningManagementDashboard.Models.Requests;
using LearningManagementDashboard.Models.Responses;
using LearningManagementDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet(Name = "GetAllCourses")]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
        {
            _logger.LogInformation("GET /api/courses called");

            var domainCourses = await _courseService.GetAllCoursesAsync();
            var courses = domainCourses.Select(c => new CourseResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });

            return Ok(courses);
        }

        [HttpGet("{id}", Name = "GetCourseById")]
        public async Task<ActionResult<CourseResponse>> GetById(Guid id)
        {
            _logger.LogInformation("GET /api/courses/{CourseId} called", id);

            var course = await _courseService.GetCourseByIdAsync(id);
            if (course is null)
                return NotFound();

            var courseResponse = new CourseResponse
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description
            };
            return Ok(courseResponse);
        }

        [HttpPost(Name = "CreateCourse")]
        public async Task<ActionResult<CourseResponse>> Create([FromBody] CreateCourseRequest req)
        {
            _logger.LogInformation("POST /api/courses (Name: {Name})", req.Name);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (await _courseService.CourseExistsByNameAsync(req.Name))
            {
                _logger.LogWarning("Conflict: course name already exists: {Name}", req.Name);
                return Conflict(new { message = "A course with that name already exists." });
            }

            var createdCourse = await _courseService.CreateCourseAsync(req.Name, req.Description);
            _logger.LogInformation("Course created: {CourseId}", createdCourse.Id);

            var courseResponse = new CourseResponse
            {
                Id = createdCourse.Id,
                Name = createdCourse.Name,
                Description = createdCourse.Description
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = courseResponse.Id },
                courseResponse
            );
        }
    }
}
