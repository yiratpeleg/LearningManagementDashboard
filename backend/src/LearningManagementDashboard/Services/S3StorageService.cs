using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace LearningManagementDashboard.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _client;
    private bool _bucketChecked;

    private const string Bucket = "learning-dashboard";

    public S3StorageService(IAmazonS3 client)
    {
        _client = client;
    }

    public async Task UploadObjectAsync(string key, Stream data)
    {
        await EnsureBucketExistsAsync();

        var req = new PutObjectRequest
        {
            BucketName = Bucket,
            Key = key,
            InputStream = data
        };
        await _client.PutObjectAsync(req);
    }

    private async Task EnsureBucketExistsAsync()
    {
        if (_bucketChecked) return;

        if (!await AmazonS3Util.DoesS3BucketExistV2Async(_client, Bucket))
        {
            await _client.PutBucketAsync(new PutBucketRequest { BucketName = Bucket });
        }

        _bucketChecked = true;
    }
}