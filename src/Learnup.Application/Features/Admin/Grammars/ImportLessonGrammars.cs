using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Admin.Grammars;

public sealed record ImportLessonGrammars(
    IReadOnlyCollection<LessonGrammarImportItem> Items) : IRequest<int>;

public sealed record LessonGrammarImportItem(
    int Level,
    int LessonOrder,
    int GrammarLevel,
    int GrammarOrder);

internal sealed class ImportLessonGrammarsHandler(ILearnupDbContext dbContext)
    : IRequestHandler<ImportLessonGrammars, int>
{
    public async Task<int> Handle(ImportLessonGrammars request, CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
        {
            return 0;
        }

        var importedCount = 0;
        var items = request.Items
            .Distinct()
            .ToList();

        ValidateGrammarLevels(items);

        foreach (var item in items)
        {
            var lesson = await dbContext.Lessons
                .FirstOrDefaultAsync(
                    lesson => lesson.CourseId == item.Level && lesson.Order == item.LessonOrder,
                    cancellationToken);

            if (lesson is null)
            {
                throw new InvalidOperationException(
                    $"Lesson with level '{item.Level}' and order '{item.LessonOrder}' was not found.");
            }

            var grammarLevel = (VocabLevel)item.GrammarLevel;
            var grammar = await dbContext.Grammars
                .FirstOrDefaultAsync(
                    grammar => grammar.Level == grammarLevel && grammar.Order == item.GrammarOrder,
                    cancellationToken);

            if (grammar is null)
            {
                throw new InvalidOperationException(
                    $"Grammar with level '{item.GrammarLevel}' and order '{item.GrammarOrder}' was not found.");
            }

            var exists = await dbContext.LessonGrammars
                .AnyAsync(
                    lessonGrammar =>
                        lessonGrammar.LessonId == lesson.Id &&
                        lessonGrammar.GrammarId == grammar.Id,
                    cancellationToken);

            if (exists)
            {
                continue;
            }

            dbContext.LessonGrammars.Add(new LessonGrammar(lesson.Id, grammar.Id));
            importedCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return importedCount;
    }

    private static void ValidateGrammarLevels(IEnumerable<LessonGrammarImportItem> items)
    {
        var invalidItem = items.FirstOrDefault(item => !Enum.IsDefined(typeof(VocabLevel), item.GrammarLevel));

        if (invalidItem is not null)
        {
            throw new InvalidOperationException(
                $"Grammar level '{invalidItem.GrammarLevel}' is not valid.");
        }
    }
}
