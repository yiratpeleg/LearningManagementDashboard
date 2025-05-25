using AutoMapper;
using LearningManagementDashboard.Models.Requests;
using LearningManagementDashboard.Models.Responses;
using LearningManagementDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        IStudentService studentService,
        IMapper mapper,
        ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetAllStudents")]
    public async Task<ActionResult<IEnumerable<StudentResponse>>> GetAll()
    {
        _logger.LogInformation("GET /api/students called");

        var students = await _studentService.GetAllStudentsAsync();
        var studentsResponse = _mapper.Map<IEnumerable<StudentResponse>>(students);

        return Ok(studentsResponse);
    }

    [HttpGet("{id}", Name = "GetStudentById")]
    public async Task<ActionResult<StudentResponse>> GetById(Guid id)
    {
        _logger.LogInformation("GET /api/students/{StudentId} called", id);

        var student = await _studentService.GetStudentByIdAsync(id);
        if (student is null)
            return NotFound();

        var studentResponse = _mapper.Map<StudentResponse>(student);
        return Ok(studentResponse);
    }

    [HttpPost(Name = "CreateStudent")]
    public async Task<ActionResult<StudentResponse>> Create(
        [FromBody] CreateStudentRequest req)
    {
        _logger.LogInformation("POST /api/students (Name: {Name})", req.FullName);

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var createdStudent = await _studentService.CreateStudentAsync(req.FullName);
        _logger.LogInformation("Student created: {StudentId}", createdStudent.Id);

        var studentResponse = _mapper.Map<StudentResponse>(createdStudent);

        return CreatedAtAction(
            nameof(GetById),
            new { id = studentResponse.Id },
            studentResponse);
    }
}