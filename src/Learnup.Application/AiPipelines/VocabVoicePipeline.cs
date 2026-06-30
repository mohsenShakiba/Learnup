using System.Diagnostics;
using FFMpegCore;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learnup.Application.AiPipelines;

public class VocabVoicePipeline(
    ILearnupDbContext dbContext,
    IVoiceProvider voiceProvider,
    IFileService fileService,
    ILogger<VocabVoicePipeline> logger) : IPipeline
{
    private const string SlowDownFilter = "atempo=0.70";

    public bool Enabled => true;

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var vocabs = await dbContext.Vocabs
            .Where(v => v.Status == VocabStatus.Published)
            .Where(vocab => vocab.VoiceId == null)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var vocab in vocabs)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                var result = await voiceProvider.GetVoiceAsync(vocab.Word, new VoiceOptions(VoiceIds.Heart, 0.9),
                    cancellationToken: cancellationToken);

                var slowedVoiceId = await SlowDownVoiceAsync(result.VoiceId, cancellationToken);

                vocab.SetVoice(slowedVoiceId);

                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Vocab voice {Vocab} was processed in {MilliSeconds}ms",
                    vocab.Word,
                    sw.ElapsedMilliseconds);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error processing vocab voice {Vocab}", vocab.Word);
            }
        }
    }

    private async Task<string> SlowDownVoiceAsync(string voiceId, CancellationToken cancellationToken)
    {
        var voiceFile = await fileService.GetAsync(voiceId, cancellationToken);

        if (voiceFile is null)
        {
            throw new InvalidOperationException($"Voice file {voiceId} was not found");
        }

        var tempDirectory = Path.Combine(Path.GetTempPath(), "learnup-vocab-voices");

        Directory.CreateDirectory(tempDirectory);

        var inputPath = Path.Combine(tempDirectory, $"{Guid.NewGuid():N}.wav");
        var outputPath = Path.Combine(tempDirectory, $"{Guid.NewGuid():N}_slow.wav");

        try
        {
            await using (voiceFile.Content)
            await using (var inputFile = File.Create(inputPath))
            {
                await voiceFile.Content.CopyToAsync(inputFile, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();

            await FFMpegArguments
                .FromFileInput(inputPath)
                .OutputToFile(outputPath, true, options => options.WithCustomArgument($"-filter:a {SlowDownFilter}"))
                .ProcessAsynchronously();

            cancellationToken.ThrowIfCancellationRequested();

            await using var outputFile = File.OpenRead(outputPath);

            return await fileService.StoreAsync(new StoreFileRequest(
                outputFile,
                $"vocab-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}-slow.wav",
                BucketNames.StoryVoices,
                "audio/wav"), cancellationToken);
        }
        finally
        {
            DeleteIfExists(inputPath);
            DeleteIfExists(outputPath);
        }
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}