using AutoMapper;
using LearningManagementDashboard.Models;
using LearningManagementDashboard.Models.Requests;
using LearningManagementDashboard.Models.Responses;
using LearningManagementDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningManagementDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly IEnrolmentService _enrolmentService;
    private readonly IMapper _mapper;
    private readonly ILogger<EnrolmentsController> _logger;

    public EnrolmentsController(
        IEnrolmentService enrolmentService,
        IMapper mapper,
        ILogger<EnrolmentsController> logger)
    {
        _enrolmentService = enrolmentService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet(Name = "GetAllEnrolments")]
    public async Task<ActionResult<IEnumerable<EnrolmentResponse>>> GetAll()
    {
        _logger.LogInformation("GET /api/enrolments called");

        var enrolments = await _enrolmentService.GetAllEnrolmentsAsync();
        var enrolmentsResponse = _mapper.Map<IEnumerable<EnrolmentResponse>>(enrolments);

        return Ok(enrolmentsResponse);
    }

    [HttpGet("student/{studentId}", Name = "GetEnrolmentsByStudent")]
    public async Task<ActionResult<IEnumerable<EnrolmentResponse>>> GetByStudent(Guid studentId)
    {
        _logger.LogInformation("GET /api/enrolments/student/{StudentId} called", studentId);

        var enrolments = await _enrolmentService.GetEnrolmentsByStudentAsync(studentId);
        var enrolmentsResponse = _mapper.Map<IEnumerable<EnrolmentResponse>>(enrolments);

        return Ok(enrolmentsResponse);
    }

    [HttpPost(Name = "CreateEnrolment")]
    public async Task<ActionResult<EnrolmentResponse>> Create(
        [FromBody] CreateEnrolmentRequest req)
    {
        _logger.LogInformation("POST /api/enrolments (Course {CourseId}, Student {StudentId})",
                               req.CourseId, req.StudentId);

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var createdEnrolment = await _enrolmentService.CreateEnrolmentAsync(req.CourseId, req.StudentId);
        _logger.LogInformation("Enrolment created: {EnrolmentId}", createdEnrolment.Id);

        var enrolmentResponse = _mapper.Map<EnrolmentResponse>(createdEnrolment);

        return CreatedAtAction(
            nameof(GetAll),
            null,
            enrolmentResponse);
    }

    [HttpGet("report", Name = "GetEnrolmentReport")]
    public async Task<ActionResult<IEnumerable<EnrolmentReportItem>>> Report()
    {
        _logger.LogInformation("GET /api/enrolments/report called");
        var report = await _enrolmentService.GenerateEnrolmentReportAsync();
        return Ok(report);
    }
}
