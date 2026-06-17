using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Application.ExternalServices;

public interface IVocabTestProvider
{
    Task<TestGenerationResult> GetVocabTestAsync(Vocab vocab, CancellationToken cancellationToken = default);
}
