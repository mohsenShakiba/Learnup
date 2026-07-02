using Learnup.API.Requests;
using Learnup.Application.Requests.Admin.Placement;
using Learnup.Application.Requests.Admin.Stories;
using Learnup.Infrastructure.ExternalService;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Admin.Controllers;

public class ImportController(
    VocabLoader vocabLoader,
    StoryLoader storyLoader,
    GrammarLoader grammarLoader,
    LessonGrammarLoader lessonGrammarLoader,
    PlacementTestLoader placementTestLoader) : BaseAdminController
{
    [HttpPost("vocabs", Name = "ImportVocabs")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> ImportVocabs(
        [FromForm] ImportVocabsRequest request,
        CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();

        try
        {
            return Ok(await vocabLoader.LoadCsvAsync(
                stream,
                request.File.FileName,
                request.LevelId,
                request.LanguageId,
                cancellationToken));
        }
        catch (Exception exception) when (exception is FormatException or InvalidOperationException or ArgumentOutOfRangeException)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("stories/{courseId:int}/{lessonOrder:int}", Name = "ImportStory")]
    public async Task<ActionResult<int>> ImportStory(
        int courseId,
        int lessonOrder,
        [FromBody] StoryRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await storyLoader.LoadAsync(
            request,
            courseId,
            lessonOrder,
            cancellationToken));
    }

    [HttpPost("placement-test", Name = "ImportPlacementTest")]
    public async Task<ActionResult<int>> ImportPlacementTest(
        [FromBody] PlacementTestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await placementTestLoader.LoadAsync(request, cancellationToken));
        }
        catch (Exception exception) when (exception is InvalidOperationException or FormatException)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("grammars")]
    public async Task<ActionResult<int>> ImportGrammar(
        [FromBody] ImportGrammarRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await grammarLoader.LoadAsync(
            request.Grammar,
            cancellationToken));
    }

    [HttpPost("lesson-grammars", Name = "ImportLessonGrammars")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> ImportLessonGrammars(
        [FromForm] ImportLessonGrammarsRequest request,
        CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();

        try
        {
            return Ok(await lessonGrammarLoader.LoadCsvAsync(
                stream,
                request.File.FileName,
                cancellationToken));
        }
        catch (Exception exception) when (exception is FormatException or InvalidOperationException)
        {
            return BadRequest(exception.Message);
        }
    }
}
