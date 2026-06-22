namespace Learnup.Application.ExternalServices;

public sealed record StoreFileRequest(
    Stream Content,
    string FileName,
    string BucketName,
    string ContentType);

public sealed record StoredFile(
    string Id,
    string ContentType);

public sealed record FileContent(
    string Id,
    Stream Content,
    string ContentType);

public interface IFileService
{
    Task<StoredFile> StoreAsync(StoreFileRequest request, CancellationToken cancellationToken);

    Task<FileContent?> GetAsync(string bucketName, string id, CancellationToken cancellationToken);
}

public class BucketNames
{
    public const string BooksBucket = "learnup-books";
}
