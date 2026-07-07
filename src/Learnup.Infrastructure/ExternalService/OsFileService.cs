using Learnup.Application.ExternalServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Learnup.Infrastructure.ExternalService;

public sealed class OsFileStorageOptions
{
    public const string SectionName = "OsFileStorage";

    public string RootPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "files");
}

internal sealed class OsFileService(IOptions<OsFileStorageOptions> options, ILogger<OsFileService> logger)
    : IFileService
{
    private readonly string rootPath = Path.GetFullPath(options.Value.RootPath);

    public async Task<string> StoreAsync(
        StoreFileRequest request,
        CancellationToken cancellationToken)
    {
        var filePath = GetSafeFilePath(request.BucketName, request.FileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using (var fileStream = new FileStream(
                         filePath,
                         FileMode.Create,
                         FileAccess.Write,
                         FileShare.None,
                         bufferSize: 81920,
                         useAsync: true))
        {
            await request.Content.CopyToAsync(fileStream, cancellationToken);
        }

        return FileIdHelper.GetFileId(request.BucketName, request.FileName);
    }

    public async Task<FileContent?> GetAsync(
        string fileId,
        CancellationToken cancellationToken)
    {
        var parsedFileId = FileIdHelper.Parse(fileId);
        if (parsedFileId is null)
        {
            return null;
        }

        var filePath = GetSafeFilePath(parsedFileId.Value.BucketName, parsedFileId.Value.Key);
        if (!File.Exists(filePath))
        {
            logger.LogInformation("file not found {File}", filePath);
            return null;
        }

        var content = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return new FileContent(fileId, content, GetContentType(filePath));
    }

    private string GetSafeFilePath(string bucketName, string key)
    {
        if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Bucket name and file key are required.");
        }

        var bucketPath = Path.GetFullPath(Path.Combine(rootPath, bucketName));
        EnsurePathIsUnderRoot(bucketPath, rootPath);

        var filePath = Path.GetFullPath(Path.Combine(bucketPath, NormalizeKey(key)));
        EnsurePathIsUnderRoot(filePath, bucketPath);

        return filePath;
    }

    private static string NormalizeKey(string key)
    {
        return key.Replace('/', Path.DirectorySeparatorChar);
    }

    private static void EnsurePathIsUnderRoot(string path, string root)
    {
        var normalizedRoot = Path.EndsInDirectorySeparator(root)
            ? root
            : root + Path.DirectorySeparatorChar;

        if (!path.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The file path resolves outside of the configured storage root.");
        }
    }

    private static string GetContentType(string filePath)
    {
        return Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".epub" => "application/epub+zip",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".wav" => "audio/wav",
            _ => "application/octet-stream"
        };
    }
}
