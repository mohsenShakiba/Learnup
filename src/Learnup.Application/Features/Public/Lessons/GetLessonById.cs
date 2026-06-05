using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Lessons;

public sealed record GetLessonById(int Id) : IRequest<LessonDetailResponse?>;

internal sealed class GetLessonByIdHandler(ILearnupDbContext dbContext)
    : IRequestHandler<GetLessonById, LessonDetailResponse?>
{
    public async Task<LessonDetailResponse?> Handle(
        GetLessonById request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Lessons
            .AsNoTracking()
            .Where(lesson => lesson.Id == request.Id)
            .Select(lesson => new LessonDetailResponse(
                lesson.Id,
                lesson.Title,
                lesson.Order,
                lesson.Status,
                lesson.CourseId,
                lesson.Stories
                    .OrderBy(lessonStory => lessonStory.StoryId)
                    .Select(lessonStory => lessonStory.StoryId)
                    .ToList(),
                lesson.Grammars
                    .OrderBy(lessonGrammar => lessonGrammar.GrammarId)
                    .Select(lessonGrammar => lessonGrammar.GrammarId)
                    .ToList(),
                lesson.Vocabs
                    .OrderBy(lessonVocab => lessonVocab.VocabId)
                    .Select(lessonVocab => lessonVocab.VocabId)
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
