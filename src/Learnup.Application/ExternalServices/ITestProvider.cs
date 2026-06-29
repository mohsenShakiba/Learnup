using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.ExternalServices;

public interface ITestProvider
{
    Task<TestGenerationResult[]> GenerateTestAsync(List<string> grammarTitles, List<string> vocabs, CancellationToken cancellationToken = default);
}
