using LearningManagementDashboard.Configuration;
using LearningManagementDashboard.Exceptions;
using LearningManagementDashboard.Mapping;
using LearningManagementDashboard.Services;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<ICourseService, CourseService>()
    .AddSingleton<IStudentService, StudentService>()
    .AddSingleton<IEnrolmentService, EnrolmentService>();

builder.Services.AddAutoMapper(typeof(CourseMappingProfile));

builder.Services.Configure<CorsSettings>(
    builder.Configuration.GetSection("CorsSettings"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var cors = builder.Configuration
                       .GetSection("CorsSettings")
                       .Get<CorsSettings>()!;

        policy.WithOrigins(cors.AllowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Map("/error", (HttpContext http) =>
{
    var feature = http.Features.Get<IExceptionHandlerFeature>();
    var ex = feature?.Error;
    var logger = http.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Unhandled exception");

    if (ex is CourseAlreadyExistsException)
    {
        return Results.Conflict(new { message = ex.Message });
    }

    return Results.Problem(
        detail: ex?.Message,
        statusCode: StatusCodes.Status500InternalServerError);
});

app.Run();
