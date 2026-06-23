using Learnup.Application.Features.Public.Files;
using Learnup.Application.Mediation;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class FilesController(IMediator mediator) : BasePublicController
{
    [HttpGet("{*id}", Name = "GetFileById")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var file = await mediator.Send(new GetFileById(id), cancellationToken);

        return file is null
            ? NotFound()
            : File(file.Content, file.ContentType, file.Id);
    }
}
