using Amazon.S3;
using Amazon.S3.Model;
using Learnup.Application.ExternalServices;

namespace Learnup.Infrastructure.ExternalService;

internal sealed class S3FileService(IAmazonS3 s3Client)
    : IFileService
{
    public async Task<StoredFile> StoreAsync(
        StoreFileRequest request,
        CancellationToken cancellationToken)
    {
        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = request.BucketName,
            Key = request.FileName,
            InputStream = request.Content,
            ContentType = request.ContentType,
        }, cancellationToken);

        return new StoredFile(request.FileName, request.ContentType);
    }

    public async Task<FileContent?> GetAsync(
        string bucketName, string id,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucketName,
                Key = id
            }, cancellationToken);

            return new FileContent(
                id,
                response.ResponseStream,
                response.Headers.ContentType);
        }
        catch
        {
            return null;
        }
    }
}
