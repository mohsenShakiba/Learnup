using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Files;
using Microsoft.Extensions.Caching.Memory;

namespace Learnup.Application.Features.Public.Files;

public sealed record GetFileById(string Id) : IRequest<FileResponse?>;

internal sealed class GetFileByIdHandler(IFileService fileService, IMemoryCache memoryCache)
    : IRequestHandler<GetFileById, FileResponse?>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<FileResponse?> Handle(GetFileById request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            return null;
        }

        if (memoryCache.TryGetValue<CachedFile>(GetCacheKey(request.Id), out var cachedFile))
        {
            return cachedFile?.ToResponse();
        }

        var file = await fileService.GetAsync(request.Id, cancellationToken);
        
        if (file is null)
        {
            return null;
        }

        var memoryStream = new MemoryStream();
        await file.Content.CopyToAsync(memoryStream, cancellationToken);
        await file.Content.DisposeAsync();

        var cacheEntry = new CachedFile(file.Id, memoryStream.ToArray(), file.ContentType);

        memoryCache.Set(GetCacheKey(request.Id), cacheEntry,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });

        return cacheEntry.ToResponse();
    }

    private static string GetCacheKey(string fileId)
    {
        return $"file:{fileId}";
    }

    private sealed record CachedFile(string Id, byte[] Content, string ContentType)
    {
        public FileResponse ToResponse()
        {
            return new FileResponse(Id, new MemoryStream(Content, writable: false), ContentType);
        }
    }
}
