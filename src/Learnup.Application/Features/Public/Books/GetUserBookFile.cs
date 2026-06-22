using Learnup.Application.ExternalServices;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Files;

namespace Learnup.Application.Features.Public.Books;

public sealed record GetUserBookFile(string FileName) : IRequest<FileResponse?>;

internal sealed class GetUserBookFileHandler(IFileService fileService)
    : IRequestHandler<GetUserBookFile, FileResponse?>
{
    public async Task<FileResponse?> Handle(
        GetUserBookFile request,
        CancellationToken cancellationToken)
    {
        
        var file = await fileService.GetAsync(BucketNames.BooksBucket, request.FileName, cancellationToken);

        return file is null
            ? null
            : new FileResponse(file.Id, file.Content, file.ContentType);
    }
}