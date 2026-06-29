using Amazon.S3;
using Amazon.S3.Model;
using Learnup.Application.ExternalServices;
using System.Net;

namespace Learnup.Infrastructure.ExternalService;

public sealed class S3FileStorageOptions
{
    public const string SectionName = "S3FileStorage";

    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public bool ForcePathStyle { get; set; }
}


internal sealed class S3FileService(IAmazonS3 s3Client)
    : IFileService
{
    public async Task<string> StoreAsync(
        StoreFileRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureBucketExistsAsync(request.BucketName, cancellationToken);

        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = request.BucketName,
            Key = request.FileName,
            InputStream = request.Content,
            ContentType = request.ContentType,
        }, cancellationToken);

        return $"{request.BucketName}/{request.FileName}";
    }

    private async Task EnsureBucketExistsAsync(
        string bucketName,
        CancellationToken cancellationToken)
    {
        try
        {
            await s3Client.HeadBucketAsync(new HeadBucketRequest
            {
                BucketName = bucketName
            }, cancellationToken);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            try
            {
                await s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName
                }, cancellationToken);
            }
            catch (AmazonS3Exception createException) when (
                createException.ErrorCode == "BucketAlreadyOwnedByYou")
            {
            }
        }
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
