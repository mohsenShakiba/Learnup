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

        return new StoredFile(GetFileId(request.BucketName, request.FileName), request.ContentType);
    }

    public async Task<FileContent?> GetAsync(
        string fileId,
        CancellationToken cancellationToken)
    {
        var parsedFileId = ParseFileId(fileId);
        if (parsedFileId is null)
        {
            return null;
        }

        try
        {
            var response = await s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = parsedFileId.Value.BucketName,
                Key = parsedFileId.Value.Key
            }, cancellationToken);

            return new FileContent(
                fileId,
                response.ResponseStream,
                response.Headers.ContentType);
        }
        catch
        {
            return null;
        }
    }

    private static string GetFileId(string bucketName, string key)
    {
        return $"{bucketName}/{key}";
    }

    private static ParsedFileId? ParseFileId(string fileId)
    {
        var parts = fileId.Split('/', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
        {
            return null;
        }

        return new ParsedFileId(parts[0], parts[1]);
    }

    private readonly record struct ParsedFileId(string BucketName, string Key);
}
