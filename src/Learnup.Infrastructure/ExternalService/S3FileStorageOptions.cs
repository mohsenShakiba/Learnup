namespace Learnup.Infrastructure.ExternalService;

public sealed class S3FileStorageOptions
{
    public const string SectionName = "S3FileStorage";

    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public bool ForcePathStyle { get; set; }
}
