// using System.Diagnostics;
// using Learnup.Application.ExternalServices;
// using Learnup.Application.Persistence;
// using Learnup.Domain.AggregateRoots.Vocabularies;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace Learnup.Application.AiPipelines;
//
// public class VocabPipeline(
//     ILearnupDbContext dbContext,
//     IVocabTranslationProvider vocabTranslationProvider,
//     ILogger<VocabPipeline> logger) : IPipeline
// {
//     public async Task ProcessAsync(CancellationToken cancellationToken = default)
//     {
//         var vocabs = await dbContext.Vocabs
//             .Where(translation => translation.Status == VocabStatus.Pending)
//             .Take(10)
//             .ToListAsync(cancellationToken);
//
//         foreach (var vocab in vocabs)
//         {
//             try
//             {
//                 var sw = Stopwatch.StartNew();
//
//                 var result = await vocabTranslationProvider.GetVocabTranslationAsync(
//                     vocab.Word,
//                     vocab.Type,
//                     cancellationToken);
//
//                 vocab.SetTranslation(
//                     result.Translation,
//                     result.Description,
//                     result.Example,
//                     result.ExampleTranslation);
//
//                 await dbContext.SaveChangesAsync(cancellationToken);
//
//                 logger.LogInformation(
//                     "Vocab translation {VocabTranslationId} for vocab {VocabId} was processed in {MilliSeconds}",
//                     vocab.Id,
//                     vocab.VocabId,
//                     sw.ElapsedMilliseconds);
//             }
//             catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
//             {
//                 throw;
//             }
//             catch (Exception exception)
//             {
//                 logger.LogError(
//                     exception,
//                     "Error processing vocab translation {VocabTranslationId} for vocab {VocabId}",
//                     vocab.Id,
//                     vocab.VocabId);
//             }
//         }
//     }
// }
