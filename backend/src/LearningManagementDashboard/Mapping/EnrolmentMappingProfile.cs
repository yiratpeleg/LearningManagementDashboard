using AutoMapper;
using LearningManagementDashboard.Models;
using LearningManagementDashboard.Models.Responses;

namespace LearningManagementDashboard.Mapping;

public class EnrolmentMappingProfile : Profile
{
    public EnrolmentMappingProfile()
    {
        CreateMap<Enrolment, EnrolmentResponse>();
    }
}
