using Learnup.Domain.AggregateRoots.Tests;

namespace Learnup.Application.ExternalServices;

public record TestGenerationResult(VocabTestType Type, string Question, TestOptionResult[] Options);

public record TestOptionResult(string Text, bool IsCorrect);
