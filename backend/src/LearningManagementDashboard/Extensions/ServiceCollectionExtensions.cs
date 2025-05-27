using Amazon.Runtime;
using Amazon.S3;
using LearningManagementDashboard.Configuration;
using LearningManagementDashboard.Mapping;
using LearningManagementDashboard.Services;

namespace LearningManagementDashboard.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS3Storage(this IServiceCollection services, IConfiguration config)
    {
        var s3ConfigSection = config.GetSection("S3");
        services.Configure<S3>(s3ConfigSection);
        var s3 = s3ConfigSection.Get<S3>()!;

        var s3Config = new AmazonS3Config
        {
            ServiceURL = s3.ServiceURL,
            ForcePathStyle = true,
            UseHttp = s3.UseHttp
        };

        var credentials = new BasicAWSCredentials(s3.AccessKey, s3.SecretKey);
        var s3Client = new AmazonS3Client(credentials, s3Config);

        services.AddSingleton<IAmazonS3>(s3Client);
        services.AddSingleton<IStorageService, S3StorageService>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddSingleton<ICourseService, CourseService>()
            .AddSingleton<IStudentService, StudentService>()
            .AddSingleton<IEnrolmentService, EnrolmentService>();

        return services;
    }

    public static IServiceCollection AddAutoMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CourseMappingProfile))
            .AddAutoMapper(typeof(EnrolmentMappingProfile))
            .AddAutoMapper(typeof(StudentMappingProfile));

        return services;
    }

    public static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration config)
    {
        var CoreConfigSection = config.GetSection("CorsSettings");
        services.Configure<CorsSettings>(CoreConfigSection);

        services.AddCors(opts =>
        {
            opts.AddDefaultPolicy(policy =>
            {
                var cors = CoreConfigSection.Get<CorsSettings>()!;

                policy.WithOrigins(cors.AllowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
}
