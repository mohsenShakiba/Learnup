using Learnup.API.Requests;
using Learnup.Application.Features.Public.Vocabs;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Vocabs;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class VocabsController(IMediator mediator) : BasePublicController
{
    [HttpPost(Name = "CreateVocab")]
    public async Task<ActionResult<int>> Create(
        [FromBody] CreateVocabRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateVocab(
            request.LanguageId,
            request.Word,
            request.Translation,
            request.Type,
            request.Level,
            request.Description,
            request.Example,
            request.ExampleTranslation,
            request.VoiceId);

        var vocabId = await mediator.Send(command, cancellationToken);

        return CreatedAtRoute(
            "GetVocabByWord",
            new { word = request.Word },
            vocabId);
    }

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
