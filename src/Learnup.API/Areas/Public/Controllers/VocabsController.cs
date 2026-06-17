using Learnup.Application.Features.Public.Vocabs;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class VocabsController(IMediator mediator) : BasePublicController
{
    [HttpGet("{word}", Name = "GetVocabByWord")]
    public async Task<ActionResult<VocabResponse>> GetByWord(
        string word,
        CancellationToken cancellationToken)
    {
        var vocab = await mediator.Send(new GetVocabByWord(word), cancellationToken);

        return vocab is null
            ? NotFound()
            : Ok(vocab);
    }
}
