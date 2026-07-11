using Learnup.Application.Features.Public.AudioBooks;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.AudioBooks;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class AudioBooksController(IMediator mediator) : BasePublicController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AudioBookResponse>>> List(
        CancellationToken cancellationToken)
    {
        var audioBooks = await mediator.Send(new ListAudioBooks(), cancellationToken);

        return Ok(audioBooks);
    }

    [HttpGet("{id:int}", Name = "GetAudioBookById")]
    public async Task<ActionResult<AudioBookDetailResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var audioBook = await mediator.Send(new GetAudioBookById(id), cancellationToken);

        return audioBook is null
            ? NotFound()
            : Ok(audioBook);
    }
}
