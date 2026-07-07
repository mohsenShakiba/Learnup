namespace Learnup.Application.ExternalServices;

public sealed record StoreFileRequest(
    Stream Content,
    string FileName,
    string BucketName,
    string ContentType);

public sealed record FileContent(
    string Id,
    Stream Content,
    string ContentType);

public interface IFileService
{
    Task<string> StoreAsync(StoreFileRequest request, CancellationToken cancellationToken);

    Task<FileContent?> GetAsync(string fileId, CancellationToken cancellationToken);
}

public class BucketNames
{
    public const string StoryVoices = "conversation_voices";
    public const string BooksBucket = "books";
    public const string BooksCoverBucket = "book_covers";
    public const string UserAvatarsBucket = "avatars";
}
