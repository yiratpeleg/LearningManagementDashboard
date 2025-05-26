namespace LearningManagementDashboard.Services;

public interface IStorageService
{
    Task UploadObjectAsync(string key, Stream data);
}
