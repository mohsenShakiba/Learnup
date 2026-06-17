namespace Learnup.Infrastructure.Prompts;

public static class VocabTestPrompt
{
    public static string GetPrompt()
    {
        return """
               You are a language test generator. Given an English word and its translation, create one multiple-choice question to test knowledge of the word.

               Rules:
               * The question should ask the user to identify the correct meaning or usage of the word.
               * Provide exactly 4 options: exactly 1 must be correct, 3 must be plausible but wrong.
               * Options should be concise (1-6 words each).
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
