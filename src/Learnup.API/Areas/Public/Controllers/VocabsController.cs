using Learnup.API.Requests;
using Learnup.Application.Features.Public.Vocabs;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class VocabsController(IMediator mediator) : BasePublicController
{
    [HttpPost(Name = "CreateVocab")]
    public async Task<ActionResult<int>> Create([FromBody] CreateVocabRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateVocab(request.LanguageId, request.Word, request.Translation);
        var vocabId = await mediator.Send(command, cancellationToken);
        return Ok(vocabId);
    }

    [HttpGet("{word}", Name = "SearchVocab")]
    public async Task<ActionResult<List<VocabResponse>>> SearchVocab(string word, CancellationToken cancellationToken)
    {
        var query = new SearchVocab(word);
        var vocabs = await mediator.Send(query, cancellationToken);
        return Ok(vocabs);
    }
}
