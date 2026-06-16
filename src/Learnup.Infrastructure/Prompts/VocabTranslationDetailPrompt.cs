namespace Learnup.Infrastructure.Prompts;

public static class VocabTranslationDetailPrompt
{
    public static string GetPrompt()
    {
        return """
               Translate the given English word into Farsi for the requested part of speech and return a valid JSON object with the following rules:
               
               * Provide the most common and appropriate Farsi translation for the requested type.
               * Include a short Farsi description that explains the meaning for that type.
               * Include one simple English example sentence using the word as the requested type.
               * Include the Farsi translation of the example sentence.
               * Return JSON only. No markdown, explanations, or additional text.
               
               Schema:
               {
                 "Translation": "string",
                 "Description": "string | null",
                 "Example": "string",
                 "ExampleTranslation": "string"
               }
               """;
    }
}
