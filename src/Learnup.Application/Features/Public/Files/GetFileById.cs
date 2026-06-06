using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Files;
using Microsoft.Extensions.Configuration;

namespace Learnup.Application.Features.Public.Files;

public sealed record GetFileById(string Id) : IRequest<FileResponse?>;

internal sealed class GetFileByIdHandler(IConfiguration configuration)
    : IRequestHandler<GetFileById, FileResponse?>
{
    public Task<FileResponse?> Handle(
        GetFileById request,
        CancellationToken cancellationToken)
    {
        var fileId = Path.GetFileName(request.Id);

        if (string.IsNullOrWhiteSpace(fileId) || fileId != request.Id)
        {
            return Task.FromResult<FileResponse?>(null);
        }

        var outputDirectory = configuration["VoiceConfiguration:OutputDirectory"];

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new InvalidOperationException("Voice output directory is not configured.");
        }

        var directoryPath = Path.GetFullPath(outputDirectory);
        var directoryRoot = Path.EndsInDirectorySeparator(directoryPath)
            ? directoryPath
            : directoryPath + Path.DirectorySeparatorChar;
        var filePath = Path.GetFullPath(Path.Combine(directoryPath, fileId));

        if (!filePath.StartsWith(directoryRoot, StringComparison.OrdinalIgnoreCase)
            || !File.Exists(filePath))
        {
            return Task.FromResult<FileResponse?>(null);
        }

        return Task.FromResult<FileResponse?>(new FileResponse(
            fileId,
            filePath,
            GetContentType(filePath)));
    }

    private static string GetContentType(string filePath)
    {
        return Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
