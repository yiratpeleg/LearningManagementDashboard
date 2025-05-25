using LearningManagementDashboard.Exceptions;
using LearningManagementDashboard.Models;
using LearningManagementDashboard.Services;
using LearningManagementDashboard.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearningManagementDashboard.Tests;

public class CourseServiceTests
{
    private readonly CourseService _service;
    private readonly Mock<ILogger<CourseService>> _loggerMock;

    public CourseServiceTests()
    {
        _loggerMock = new Mock<ILogger<CourseService>>();
        _service = new CourseService(_loggerMock.Object);
    }

    [Fact]
    public async Task GetAllCoursesAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        // Act
        var all = await _service.GetAllCoursesAsync();

        // Assert
        Assert.Empty(all);
        _loggerMock.VerifyLog(LogLevel.Information, "Getting all courses", Times.Once());
    }

    [Fact]
    public async Task GetAllCoursesAsync_AfterAddingMultiple_ReturnsAllCourses()
    {
        // Arrange
        var c1 = await _service.CreateCourseAsync("A", "a");
        var c2 = await _service.CreateCourseAsync("B", "b");

        // Act
        var all = (await _service.GetAllCoursesAsync()).ToList();

        // Assert
        Assert.Equal(2, all.Count);
        Assert.Contains(all, c => c.Id == c1.Id && c.Name == "A");
        Assert.Contains(all, c => c.Id == c2.Id && c.Name == "B");
        _loggerMock.VerifyLog(LogLevel.Information, "Getting all courses", Times.Once());
    }

    [Fact]
    public async Task GetCourseByIdAsync_ExistingId_ReturnsCourse()
    {
        // Arrange
        var created = await _service.CreateCourseAsync("Name", "Description");

        // Act
        var fetched = await _service.GetCourseByIdAsync(created.Id);

        // Assert
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);

        _loggerMock.VerifyLog(LogLevel.Information, "Getting course by Id", Times.Once());
    }

    [Fact]
    public async Task GetCourseByIdAsync_NonExistingId_ReturnsNullAndLogsWarning()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var fetched = await _service.GetCourseByIdAsync(id);

        // Assert
        Assert.Null(fetched);
        _loggerMock.VerifyLog(LogLevel.Warning, "Course not found", Times.Once());
    }

    [Fact]
    public async Task CreateCourseAsync_ValidName_CreatesAndLogsInformation()
    {
        // Arrange
        var name = "TestCourse";
        var desc = "TestDesc";

        // Act
        var course = await _service.CreateCourseAsync(name, desc);

        // Assert
        Assert.NotNull(course);
        Assert.Equal(name, course.Name);
        Assert.Equal(desc, course.Description);

        _loggerMock.VerifyLog(LogLevel.Information, "Created course", Times.Once());
    }

    [Fact]
    public async Task CreateCourseAsync_DuplicateName_ThrowsAndLogsWarning()
    {
        // Arrange
        var name = "Dup";

        // Act & Assert
        await _service.CreateCourseAsync(name, "D1");
        await Assert.ThrowsAsync<CourseAlreadyExistsException>(
            () => _service.CreateCourseAsync(name, "D2")
        );

        _loggerMock.VerifyLog(LogLevel.Warning, "Course name conflict", Times.Once());
    }

    [Fact]
    public async Task UpdateCourseAsync_ExistingCourse_UpdatesAndLogsInfo()
    {
        // Arrange
        var course = await _service.CreateCourseAsync("OldName", "OldDescription");
        course.Name = "NewName";
        course.Description = "NewDescription";

        // Act
        await _service.UpdateCourseAsync(course);

        // Assert
        var updated = await _service.GetCourseByIdAsync(course.Id);

        Assert.Equal("NewName", updated!.Name);
        Assert.Equal("NewDescription", updated.Description);

        _loggerMock.VerifyLog(LogLevel.Information, "Course " + course.Id + " updated", Times.Once());
    }

    [Fact]
    public async Task UpdateCourseAsync_NonExistingCourse_ThrowsKeyNotFoundAndLogsError()
    {
        // Arrange
        var fake = new Course { Id = Guid.NewGuid(), Name = "Nope", Description = "none" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateCourseAsync(fake));
        _loggerMock.VerifyLog(LogLevel.Error, "Failed to update: Course not found", Times.Once());
    }

    [Fact]
    public async Task DeleteCourseAsync_ExistingCourse_RemovesAndLogsInfo()
    {
        // Arrange
        var course = await _service.CreateCourseAsync("ToDelete", "delete");

        // Act
        await _service.DeleteCourseAsync(course.Id);

        // Assert
        var fetched = await _service.GetCourseByIdAsync(course.Id);
        Assert.Null(fetched);

        _loggerMock.VerifyLog(LogLevel.Information, "Course " + course.Id + " deleted", Times.Once());
    }

    [Fact]
    public async Task DeleteCourseAsync_NonExistingCourse_ThrowsKeyNotFoundAndLogsError()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteCourseAsync(id));
        _loggerMock.VerifyLog(LogLevel.Error, "Failed to delete: Course not found", Times.Once());
    }
}
