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
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> ImportStory(
        int courseId,
        int lessonOrder,
        [FromForm] ImportStoryRequest request,
        CancellationToken cancellationToken)
    {
        StoryRequest storyRequest;

        await using (var stream = request.File.OpenReadStream())
        using (var reader = new StreamReader(stream))
        {
            var content = await reader.ReadToEndAsync(cancellationToken);
            storyRequest = ParseStory(content);
        }

        try
        {
            return Ok(await storyLoader.LoadAsync(
                storyRequest,
                courseId,
                lessonOrder,
                cancellationToken));
        }
        catch (Exception exception) when (exception is InvalidOperationException or FormatException)
        {
            return BadRequest(exception.Message);
        }
    }

    // Expected txt format:
    //   line 1: story title
    //   line 2: comma separated words
    //   line 3+: one sentence per line
    // Order and person are inferred from the sentence position, speakers are
    // assumed to alternate. Translation and description are intentionally left null.
    private static StoryRequest ParseStory(string content)
    {
        var lines = content
            .Split('\n')
            .Select(line => line.Trim('\r', ' ', '\t'))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (lines.Count < 3)
        {
            throw new FormatException(
                "Story file must contain a title, a line of words and at least one sentence.");
        }

        var title = lines[0];

        var words = lines[1]
            .Split(',')
            .Select(word => word.Trim())
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .ToList();

        var sentences = lines
            .Skip(2)
            .Select((text, index) => new StoryItemRequest(
                Order: index + 1,
                Text: text,
                Person: index % 2 == 0 ? 1 : 2,
                Translation: null))
            .ToList();

        return new StoryRequest(title, words, sentences);
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
