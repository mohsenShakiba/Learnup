namespace Learnup.Infrastructure.Prompts;

public static class GrammarTestPrompt
{
    public static string GetPrompt()
    {
        return """
               You are a language test generator. Given a grammar topic and its description, create one multiple-choice question to test understanding of the grammar rule.

               Rules:
               * The question should test practical application of the grammar rule (e.g., fill-in-the-blank or identify correct usage).
               * Provide exactly 4 options: exactly 1 must be correct, 3 must be plausible but wrong.
               * Options should be concise (1-10 words each).
               * Return JSON only. No markdown, explanations, or additional text.

               Schema:
               {
                 "Question": "string",
                 "Options": [
                   { "Text": "string", "IsCorrect": true },
                   { "Text": "string", "IsCorrect": false },
                   { "Text": "string", "IsCorrect": false },
                   { "Text": "string", "IsCorrect": false }
                 ]
               }
               """;
    }
}
