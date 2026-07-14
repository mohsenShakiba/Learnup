using Learnup.API.Requests;
using Learnup.Application.Requests.Admin.AudioBooks;
using Learnup.Application.Requests.Admin.Placement;
using Learnup.Application.Requests.Admin.Conversations;
using Learnup.Infrastructure.ExternalService;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Learnup.API.Areas.Admin.Controllers;

public class ImportController(
    VocabLoader vocabLoader,
    AudioBookLoader audioBookLoader,
    ConversationLoader conversationLoader,
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

    [HttpPost("audio-books", Name = "ImportAudioBook")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> ImportAudioBook(
        [FromForm] ImportAudioBookRequest request,
        CancellationToken cancellationToken)
    {
        AudioBookImportRequest audioBookRequest;

        await using (var stream = request.File.OpenReadStream())
        using (var reader = new StreamReader(stream))
        {
            var content = await reader.ReadToEndAsync(cancellationToken);
            audioBookRequest = ParseAudioBook(content);
        }

        try
        {
            return Ok(await audioBookLoader.LoadAsync(audioBookRequest, cancellationToken));
        }
        catch (Exception exception) when (exception is InvalidOperationException or FormatException)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPost("conversations/{courseId:int}/{lessonOrder:int}", Name = "ImportConversation")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> ImportConversation(
        int courseId,
        int lessonOrder,
        [FromForm] ImportConversationRequest request,
        CancellationToken cancellationToken)
    {
        ConversationRequest conversationRequest;

        await using (var stream = request.File.OpenReadStream())
        using (var reader = new StreamReader(stream))
        {
            var content = await reader.ReadToEndAsync(cancellationToken);
            conversationRequest = ParseConversation(content);
        }

        try
        {
            return Ok(await conversationLoader.LoadAsync(
                conversationRequest,
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
    //   line 1: conversation title
    //   line 2+: one sentence per line
    // Order and person are inferred from the sentence position, speakers are
    // assumed to alternate. Words are inferred from the sentences.
    // Translation and description are intentionally left null.
    private static ConversationRequest ParseConversation(string content)
    {
        var lines = content
            .Split('\n')
            .Select(line => line.Trim('\r', ' ', '\t'))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (lines.Count < 2)
        {
            throw new FormatException(
                "Conversation file must contain a title and at least one sentence.");
        }

        var title = lines[0];

        var sentences = lines
            .Skip(1)
            .Select((text, index) => new ConversationItemRequest(
                Order: index + 1,
                Text: text,
                Person: index % 2 == 0 ? 1 : 2,
                Translation: null))
            .ToList();

        var words = ExtractConversationWords(sentences.Select(sentence => sentence.Text));

        return new ConversationRequest(title, words, sentences);
    }

    private static List<string> ExtractConversationWords(IEnumerable<string> sentences)
    {
        return sentences
            .SelectMany(sentence => Regex.Matches(sentence, @"[\p{L}]+(?:['’][\p{L}]+)?")
                .Select(match => match.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static AudioBookImportRequest ParseAudioBook(string content)
    {
        var lines = content
            .Replace("\r\n", "\n")
            .Replace('\r', '\n')
            .Split('\n')
            .Select(line => line.Trim())
            .ToList();

        var separatorIndex = lines.FindIndex(line => line.Length >= 5 && line.All(character => character == '-'));

        if (separatorIndex < 0)
        {
            throw new FormatException("Audio book file must contain a dashed separator line before the book content.");
        }

        var headerLines = lines.Take(separatorIndex).ToList();
        var contentLines = lines.Skip(separatorIndex + 1).ToList();
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var descriptionLines = new List<string>();
        var isReadingDescription = false;

        foreach (var line in headerLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("توضیح:", StringComparison.Ordinal))
            {
                isReadingDescription = true;
                descriptionLines.Add(line["توضیح:".Length..].Trim());
                continue;
            }

            if (isReadingDescription)
            {
                descriptionLines.Add(line);
                continue;
            }

            var colonIndex = line.IndexOf(':');
            if (colonIndex <= 0)
            {
                continue;
            }

            var key = line[..colonIndex].Trim();
            var value = line[(colonIndex + 1)..].Trim();

            if (!string.IsNullOrWhiteSpace(key))
            {
                metadata[key] = value;
            }
        }

        var title = GetRequiredValue(metadata, "Title");
        var items = SplitIntoAudioBookItems(contentLines);
        var body = NormalizeParagraphs(items.Select(item => item.Sentence));

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new FormatException("Audio book content is required after the dashed separator line.");
        }

        return new AudioBookImportRequest(
            title,
            NormalizeParagraphs(descriptionLines),
            GetOptionalValue(metadata, "Author"),
            GetOptionalValue(metadata, "First published", "Year"),
            GetOptionalValue(metadata, "Level (approx.)", "Level"),
            GetOptionalValue(metadata, "Word count"),
            GetOptionalValue(metadata, "Source"),
            body,
            items);
    }

    private static string GetRequiredValue(Dictionary<string, string> metadata, string key)
    {
        return metadata.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new FormatException($"Audio book metadata '{key}' is required.");
    }

    private static string? GetOptionalValue(Dictionary<string, string> metadata, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (metadata.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static string NormalizeParagraphs(IEnumerable<string> lines)
    {
        return string.Join(
            Environment.NewLine + Environment.NewLine,
            SplitIntoParagraphItems(lines));
    }

    private static IReadOnlyList<string> SplitIntoParagraphItems(IEnumerable<string> lines)
    {
        var items = new List<string>();
        var currentLines = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                AddCurrentParagraph();
                continue;
            }

            currentLines.Add(line.Trim());
        }

        AddCurrentParagraph();

        return items;
        void AddCurrentParagraph()
        {
            if (currentLines.Count == 0)
            {
                return;
            }

            items.Add(string.Join(' ', currentLines));
            currentLines.Clear();
        }
    }

    private static IReadOnlyList<AudioBookImportItemRequest> SplitIntoAudioBookItems(IEnumerable<string> lines)
    {
        var nonEmptyLines = lines
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        var items = new List<AudioBookImportItemRequest>();

        for (var index = 0; index < nonEmptyLines.Count; index++)
        {
            var sentence = nonEmptyLines[index];
            string? translation = null;

            if (index + 1 < nonEmptyLines.Count && ContainsPersianLetter(nonEmptyLines[index + 1]))
            {
                translation = nonEmptyLines[index + 1];
                index++;
            }

            items.Add(new AudioBookImportItemRequest(sentence, translation));
        }

        return items;
    }

    private static bool ContainsPersianLetter(string value)
    {
        return value.Any(character =>
            character is >= '\u0600' and <= '\u06FF'
            or >= '\u0750' and <= '\u077F'
            or >= '\u08A0' and <= '\u08FF'
            or >= '\uFB50' and <= '\uFDFF'
            or >= '\uFE70' and <= '\uFEFF');
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
