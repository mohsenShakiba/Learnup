using Learnup.Application.ExternalServices;
using Learnup.Application.Requests.Admin.Conversations;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Conversations;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public class ConversationLoader(LearnupDbContext dbContext) 
{
    public async Task<int> LoadAsync(
        ConversationRequest conversationRequest,
        int courseId,
        int lessonOrder,
        CancellationToken cancellationToken = default)
    {
        var course = await dbContext.Courses
            .Where(currentCourse => currentCourse.Id == courseId)
            .SingleOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            throw new InvalidOperationException($"Course with id '{courseId}' was not found.");
        }

        ValidateConversation(conversationRequest);
        ValidateLessonOrder(lessonOrder);

        var words = conversationRequest.Words;
        var sentences = conversationRequest.Sentences;

        var normalizedWords = words
            .Select(word => word.Trim())
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var lesson = new Lesson(conversationRequest.Title.Trim(), lessonOrder, courseId);
        dbContext.Lessons.Add(lesson);
        await dbContext.SaveChangesAsync(cancellationToken);

        var vocabIds = await EnsureVocabsAsync(normalizedWords, course.LanguageId, cancellationToken);
        var conversation = new Conversation(conversationRequest.Title.Trim());

        foreach (var sentence in sentences.OrderBy(sentence => sentence.Order))
        {
            conversation.Items.Add(new ConversationItem(sentence.Text.Trim(), sentence.Translation?.Trim(), sentence.Person, sentence.Order));
        }

        dbContext.Conversations.Add(conversation);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.Set<LessonConversation>().Add(new LessonConversation(lesson.Id, conversation.Id));
        
        await AddMissingLessonVocabLinksAsync(lesson.Id, vocabIds, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);

        return conversation.Id;
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
            .Select(word => new Vocab(languageId, word, VocabLevel.Unknown))
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

    private static void ValidateConversation(ConversationRequest conversation)
    {
        if (string.IsNullOrWhiteSpace(conversation.Title))
        {
            throw new InvalidOperationException("Conversation title is required.");
        }

        if (conversation.Words is null)
        {
            throw new InvalidOperationException("Conversation words are required.");
        }

        if (conversation.Sentences is null)
        {
            throw new InvalidOperationException("Conversation sentences are required.");
        }

        if (conversation.Sentences.Count == 0)
        {
            throw new InvalidOperationException("Conversation must contain at least one sentence.");
        }

        var invalidSentence = conversation.Sentences
            .FirstOrDefault(sentence => string.IsNullOrWhiteSpace(sentence.Text));

        if (invalidSentence is not null)
        {
            throw new InvalidOperationException($"Conversation sentence at order '{invalidSentence.Order}' is invalid.");
        }
    }

    private static void ValidateLessonOrder(int lessonOrder)
    {
        if (lessonOrder <= 0)
        {
            throw new InvalidOperationException("Lesson order must be greater than zero.");
        }
    }
}
