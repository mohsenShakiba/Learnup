namespace Learnup.Application.ExternalServices;

public record TestGenerationResult(string Question, TestOptionResult[] Options);

public record TestOptionResult(string Text, bool IsCorrect);
