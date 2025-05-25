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
    .AddSingleton<IStudentService, StudentService>();

builder.Services.AddAutoMapper(typeof(CourseMappingProfile));

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
