using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Files;

namespace Learnup.Application.Features.Public.Files;

public sealed record GetFileById(string Id) : IRequest<FileResponse?>;

internal sealed class GetFileByIdHandler(IFileService fileService)
    : IRequestHandler<GetFileById, FileResponse?>
{
    public async Task<FileResponse?> Handle(
        GetFileById request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            return null;
        }

        var file = await fileService.GetAsync(request.Id, cancellationToken);

        return file is null
            ? null
            : new FileResponse(file.Id, file.Content, file.ContentType);
    }
}
