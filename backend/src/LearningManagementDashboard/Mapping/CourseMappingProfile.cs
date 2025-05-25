using AutoMapper;
using LearningManagementDashboard.Models;
using LearningManagementDashboard.Models.Requests;
using LearningManagementDashboard.Models.Responses;

namespace LearningManagementDashboard.Mapping;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseResponse>();
        CreateMap<CreateCourseRequest, Course>();
    }
}
