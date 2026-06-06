using System.Text;
using Learnup.API.Requests;
using Learnup.API.Responses;
using Learnup.Application.Features.Admin.Grammars;
using Learnup.Application.ExternalServices;
using Learnup.Application.Features.Admin;
using Learnup.Application.Mediation;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Admin.Controllers;


public class ImportController(
    IVocabLoader vocabLoader,
    IMediator mediator) : BaseAdminController
{
    [HttpPost("vocabs", Name = "ImportVocabs")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImportVocabsResponse>> ImportVocabs(
        [FromForm] ImportVocabsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File.Length == 0)
        {
            return BadRequest("CSV file is empty.");
        }

        if (!Path.GetExtension(request.File.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only CSV files are supported.");
        }

        await using var stream = request.File.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var words = await ReadWordsAsync(reader, cancellationToken);

        if (words.Count == 0)
        {
            return BadRequest("CSV file does not contain any vocab words.");
        }

        var importedCount = await vocabLoader.LoadAsync(
            words.ToArray(),
            request.LevelId,
            request.LanguageId,
            cancellationToken);

        return Ok(new ImportVocabsResponse(words.Count, importedCount));
    }

    [HttpPost("stories", Name = "ImportStory")]
    public async Task<ActionResult<int>> ImportStory(
        [FromBody] ImportStoryRequest request,
        CancellationToken cancellationToken)
    {
        var storyId = await mediator.Send(
            new ImportStory(
                request.Story,
                request.CourseId,
                request.LessonId,
                request.GrammarIds),
            cancellationToken);

        return Ok(storyId);
    }

    [HttpPost("grammars")]
    public async Task<ActionResult<int>> ImportGrammar(
        [FromBody] ImportGrammarRequest request,
        CancellationToken cancellationToken)
    {
        var grammarId = await mediator.Send(
            new ImportGrammar(request.Grammar),
            cancellationToken);

        return Ok(grammarId);
    }

    private static async Task<List<string>> ReadWordsAsync(
        TextReader reader,
        CancellationToken cancellationToken)
    {
        var words = new List<string>();
        var isFirstRow = true;

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            var columns = ParseCsvLine(line);
            if (columns.Count == 0)
            {
                continue;
            }

            var word = columns[0].Trim();
            if (string.IsNullOrWhiteSpace(word))
            {
                continue;
            }

            if (isFirstRow && word.Equals("word", StringComparison.OrdinalIgnoreCase))
            {
                isFirstRow = false;
                continue;
            }

            isFirstRow = false;
            words.Add(word);
        }

        return words;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var columns = new List<string>();
        var value = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var current = line[index];

            if (current == '"')
            {
                if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    value.Append('"');
                    index++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (current == ',' && !inQuotes)
            {
                columns.Add(value.ToString());
                value.Clear();
                continue;
            }

            value.Append(current);
        }

        columns.Add(value.ToString());
        return columns;
    }
}
