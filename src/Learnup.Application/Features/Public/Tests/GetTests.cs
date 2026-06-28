// using Learnup.Application.Authentication;
// using Learnup.Application.Mediation;
// using Learnup.Application.Persistence;
// using Learnup.Application.Responses.Public.Tests;
// using Learnup.Domain.AggregateRoots.Tests;
// using Microsoft.EntityFrameworkCore;
//
// namespace Learnup.Application.Features.Public.Tests;
//
// public sealed record GetTests(int LessonId, TestType Type) : IRequest<IReadOnlyList<TestResponse>>;
//
// internal sealed class GetTestsHandler(ILearnupDbContext dbContext, IIdentityProvider identityProvider)
//     : IRequestHandler<GetTests, IReadOnlyList<TestResponse>>
// {
//     public async Task<IReadOnlyList<TestResponse>> Handle(GetTests request, CancellationToken cancellationToken)
//     {
//         var tests = await dbContext.Tests
//             .AsNoTracking()
//             .Include(t => t.Options)
//             .Where(t => t.LessonId == request.LessonId
//                 && t.Type == request.Type
//                 && t.Status == TestStatus.Published)
//             .ToListAsync(cancellationToken);
//
//         var testIds = tests.Select(t => t.Id).ToList();
//
//         var userResults = await dbContext.UserTestResults
//             .AsNoTracking()
//             .Where(r => r.UserId == identityProvider.UserId && testIds.Contains(r.TestId))
//             .ToListAsync(cancellationToken);
//
//         return tests.Select(t =>
//         {
//             var result = userResults.FirstOrDefault(r => r.TestId == t.Id);
//             return new TestResponse(
//                 t.Id,
//                 t.LessonId,
//                 t.Type,
//                 t.QuestionType,
//                 t.Question,
//                 t.Options.Select(o => new TestOptionResponse(o.Id, o.Text)).ToList(),
//                 result?.SelectedOptionId,
//                 result?.IsCorrect);
//         }).ToList();
//     }
// }
