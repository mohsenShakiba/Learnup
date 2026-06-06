using Learnup.Application.ExternalServices;
using Learnup.Application.Requests.Admin.Grammars;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.ExternalService;

public class GrammarLoader(LearnupDbContext dbContext) : IGrammarLoader
{
    public async Task<int> LoadAsync(
        GrammarRequest grammarRequest,
        CancellationToken cancellationToken = default)
    {
        ValidateGrammar(grammarRequest);

        if (!Enum.IsDefined(typeof(VocabLevel), grammarRequest.LevelId))
        {
            throw new ArgumentOutOfRangeException(
                nameof(grammarRequest.LevelId),
                grammarRequest.LevelId,
                "Unknown grammar level id.");
        }

        Grammar? parentGrammar = null;
        if (grammarRequest.ParentGrammarId.HasValue)
        {
            parentGrammar = await dbContext.Grammars
                .FirstOrDefaultAsync(
                    grammar => grammar.Id == grammarRequest.ParentGrammarId.Value,
                    cancellationToken);

            if (parentGrammar is null)
            {
                throw new InvalidOperationException(
                    $"Parent grammar with id '{grammarRequest.ParentGrammarId.Value}' was not found.");
            }
        }

        var grammar = new Grammar(
            0,
            grammarRequest.Name.Trim(),
            (VocabLevel)grammarRequest.LevelId,
            grammarRequest.Order,
            TimeSpan.FromMinutes(grammarRequest.EstimatedTimeMinutes),
            grammarRequest.Description.Trim(),
            grammarRequest.ParentGrammarId);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        dbContext.Grammars.Add(grammar);

        if (parentGrammar is not null)
        {
            parentGrammar.AddGrammar(grammar);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var lessonRequest in grammarRequest.Lessons.OrderBy(lesson => lesson.Order))
        {
            grammar.AddLesson(new GrammarLesson(
                0,
                lessonRequest.Title.Trim(),
                lessonRequest.HtmlTag,
                lessonRequest.Content.Trim(),
                lessonRequest.Order,
                lessonRequest.Language.Trim(),
                lessonRequest.VoiceId,
                grammar.Id));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return grammar.Id;
    }

    private static void ValidateGrammar(GrammarRequest grammarRequest)
    {
        if (string.IsNullOrWhiteSpace(grammarRequest.Name))
        {
            throw new InvalidOperationException("Grammar name is required.");
        }

        if (string.IsNullOrWhiteSpace(grammarRequest.Description))
        {
            throw new InvalidOperationException("Grammar description is required.");
        }

        if (grammarRequest.Order < 0)
        {
            throw new InvalidOperationException("Grammar order must be greater than or equal to zero.");
        }

        if (grammarRequest.EstimatedTimeMinutes < 0)
        {
            throw new InvalidOperationException("Grammar estimated time must be greater than or equal to zero.");
        }

        if (grammarRequest.Lessons is null)
        {
            throw new InvalidOperationException("Grammar lessons are required.");
        }

        var duplicateLessonOrder = grammarRequest.Lessons
            .GroupBy(lesson => lesson.Order)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateLessonOrder is not null)
        {
            throw new InvalidOperationException(
                $"Grammar lesson order '{duplicateLessonOrder.Key}' is duplicated.");
        }

        var invalidLesson = grammarRequest.Lessons
            .FirstOrDefault(lesson =>
                string.IsNullOrWhiteSpace(lesson.Title) ||
                string.IsNullOrWhiteSpace(lesson.Content) ||
                string.IsNullOrWhiteSpace(lesson.Language) ||
                lesson.Order < 0);

        if (invalidLesson is not null)
        {
            throw new InvalidOperationException(
                $"Grammar lesson at order '{invalidLesson.Order}' is invalid.");
        }
    }
}
