using Learnup.Application.Features.Public.Vocabs;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class VocabsController(IMediator mediator) : BasePublicController
{
    [HttpGet("{word}", Name = "GetVocabByWord")]
    public async Task<ActionResult<List<VocabResponse>>> GetByWord(
        string word,
        CancellationToken cancellationToken)
    {
        var query = new GetVocabByWord(word);
        var vocabs = await mediator.Send(query, cancellationToken);
        return Ok(vocabs);
    }
}
