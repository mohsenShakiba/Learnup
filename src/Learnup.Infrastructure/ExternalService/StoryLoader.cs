using Learnup.Application.ExternalServices;
using Learnup.Application.Requests.Admin.Stories;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Stories;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public class StoryLoader(LearnupDbContext dbContext) : IStoryLoader
{
    public async Task<int> LoadAsync(
        StoryRequest storyRequest,
        int courseId,
        int lessonId,
        List<int> grammarIds,
        CancellationToken cancellationToken = default)
    {
        var lesson = await dbContext.Lessons
            .Include(l => l.Course)
            .Where(currentLesson => currentLesson.Id == lessonId)
            .SingleOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            throw new InvalidOperationException($"Lesson with id '{lessonId}' was not found.");
        }

        if (lesson.CourseId != courseId)
        {
            throw new InvalidOperationException($"Lesson '{lessonId}' does not belong to course '{courseId}'.");
        }

        ValidateStory(storyRequest);

        var words = storyRequest.Words;
        var sentences = storyRequest.Sentences;

        await ValidateGrammarsAsync(grammarIds, cancellationToken);

        var normalizedWords = words
            .Select(word => word.Trim())
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var vocabIds = await EnsureVocabsAsync(normalizedWords, lesson.Course.LanguageId, cancellationToken);
        var story = new Story(storyRequest.Title.Trim());

        foreach (var sentence in sentences.OrderBy(sentence => sentence.Order))
        {
            story.Items.Add(new StoryItem(sentence.Text.Trim(), sentence.Translation.Trim(), sentence.Order));
        }

        dbContext.Stories.Add(story);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Set<LessonStory>().Add(new LessonStory(lessonId, story.Id));
        
        await AddMissingLessonVocabLinksAsync(lessonId, vocabIds, cancellationToken);
        await AddMissingLessonGrammarLinksAsync(lessonId, grammarIds, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);

        return story.Id;
    }

    private async Task<List<int>> EnsureVocabsAsync(
        List<string> normalizedWords,
        int languageId,
        CancellationToken cancellationToken)
    {
        if (normalizedWords.Count == 0)
        {
            return [];
        }

        var lowerWords = normalizedWords
            .Select(word => word.ToLowerInvariant())
            .ToList();

        var existingVocabs = await dbContext.Vocabs
            .Where(vocab => vocab.LanguageId == languageId && lowerWords.Contains(vocab.Word.ToLower()))
            .Select(vocab => new { vocab.Id, vocab.Word })
            .ToListAsync(cancellationToken);

        var existingWordSet = existingVocabs
            .Select(vocab => vocab.Word)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newVocabs = normalizedWords
            .Where(word => !existingWordSet.Contains(word))
            .Select(word => new Vocab(languageId, word, VocabLevel.Common))
            .ToList();

        if (newVocabs.Count > 0)
        {
            dbContext.Vocabs.AddRange(newVocabs);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return existingVocabs
            .Select(vocab => vocab.Id)
            .Concat(newVocabs.Select(vocab => vocab.Id))
            .ToList();
    }

    private async Task AddMissingLessonVocabLinksAsync(
        int lessonId,
        List<int> vocabIds,
        CancellationToken cancellationToken)
    {
        if (vocabIds.Count == 0)
        {
            return;
        }

        var existingVocabIds = await dbContext.Set<LessonVocab>()
            .Where(lessonVocab => lessonVocab.LessonId == lessonId && vocabIds.Contains(lessonVocab.VocabId))
            .Select(lessonVocab => lessonVocab.VocabId)
            .ToListAsync(cancellationToken);

        var existingVocabIdSet = existingVocabIds.ToHashSet();
        var newLessonVocabs = vocabIds
            .Where(vocabId => !existingVocabIdSet.Contains(vocabId))
            .Select(vocabId => new LessonVocab(lessonId, vocabId));

        dbContext.Set<LessonVocab>().AddRange(newLessonVocabs);
    }

    private async Task AddMissingLessonGrammarLinksAsync(
        int lessonId,
        List<int> grammarIds,
        CancellationToken cancellationToken)
    {
        if (grammarIds.Count == 0)
        {
            return;
        }

        var existingGrammarIds = await dbContext.Set<LessonGrammar>()
            .Where(lessonGrammar => lessonGrammar.LessonId == lessonId && grammarIds.Contains(lessonGrammar.GrammarId))
            .Select(lessonGrammar => lessonGrammar.GrammarId)
            .ToListAsync(cancellationToken);

        var existingGrammarIdSet = existingGrammarIds.ToHashSet();
        var newLessonGrammars = grammarIds
            .Where(grammarId => !existingGrammarIdSet.Contains(grammarId))
            .Select(grammarId => new LessonGrammar(lessonId, grammarId));

        dbContext.Set<LessonGrammar>().AddRange(newLessonGrammars);
    }

    private async Task ValidateGrammarsAsync(
        List<int> grammarIds,
        CancellationToken cancellationToken)
    {
        if (grammarIds.Count == 0)
        {
            return;
        }

        var existingGrammarIds = await dbContext.Grammars
            .Where(grammar => grammarIds.Contains(grammar.Id))
            .Select(grammar => grammar.Id)
            .ToListAsync(cancellationToken);

        var missingGrammarIds = grammarIds
            .Except(existingGrammarIds)
            .ToList();

        if (missingGrammarIds.Count > 0)
        {
            throw new InvalidOperationException($"Grammar ids were not found: {string.Join(", ", missingGrammarIds)}.");
        }
    }

    private static void ValidateStory(StoryRequest story)
    {
        if (string.IsNullOrWhiteSpace(story.Title))
        {
            throw new InvalidOperationException("Story title is required.");
        }

        if (story.Words is null)
        {
            throw new InvalidOperationException("Story words are required.");
        }

        if (story.Sentences is null)
        {
            throw new InvalidOperationException("Story sentences are required.");
        }

        if (story.Sentences.Count == 0)
        {
            throw new InvalidOperationException("Story must contain at least one sentence.");
        }

        var invalidSentence = story.Sentences
            .FirstOrDefault(sentence =>
                string.IsNullOrWhiteSpace(sentence.Text) ||
                string.IsNullOrWhiteSpace(sentence.Translation));

        if (invalidSentence is not null)
        {
            throw new InvalidOperationException($"Story sentence at order '{invalidSentence.Order}' is invalid.");
        }
    }
}
