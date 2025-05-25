using AutoMapper;
using LearningManagementDashboard.Models;
using LearningManagementDashboard.Models.Requests;
using LearningManagementDashboard.Models.Responses;

namespace LearningManagementDashboard.Mapping;

public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentResponse>();
        CreateMap<CreateStudentRequest, Student>();
    }
}
