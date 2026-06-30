using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.API.Requests;

public sealed record CreateVocabRequest(
    int LanguageId,
    string Word);
