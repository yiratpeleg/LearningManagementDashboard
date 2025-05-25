using LearningManagementDashboard.Mapping;
using LearningManagementDashboard.Services;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<ICourseService, CourseService>();

builder.Services.AddAutoMapper(typeof(CourseMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
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

    return Results.Problem(
        detail: ex?.Message,
        statusCode: StatusCodes.Status500InternalServerError);
});

app.Run();
