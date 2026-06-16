namespace Learnup.Infrastructure.Prompts;

public static class VocabTranslationPrompt
{
    public static string GetPrompt()
    {
        return """
               Translate the given English word into Farsi and return a valid JSON object with the following rules:
               
               * Provide the most common and appropriate Farsi translation.
               * Include a short description only when necessary; otherwise use `null`.
               * If the word is an inflected form (e.g., plural noun, past tense verb), provide its base form in `ParentWord`; otherwise use `null`.
               * Identify whether the word is commonly used as a **Noun**, **Verb**, or both.
               * Only use the types `"Noun"` and `"Verb"`.
               * Do not include rare, archaic, or highly unlikely parts of speech (e.g., "ticket" as a verb, "pen" as a verb, "buy" as a noun).
               * For each applicable type, provide:
                 * Farsi translation
                 * Optional description (`null` if not needed)
                 * One natural English example sentence
                 * The Farsi translation of that example sentence
               * If the word is only a noun, return exactly one `"Noun"` entry.
               * If the word is only a verb, return exactly one `"Verb"` entry.
               * If the word is commonly both a noun and a verb, return both entries.
               
               Return JSON only. No markdown, explanations, or additional text.
               Schema:
               {
                 "Translation": "string",
                 "Description": "string | null",
                 "ParentWord": "string | null",
                 "Transactions": [
                   {
                     "Type": "Noun | Verb",
                     "Translation": "string",
                     "Description": "string | null",
                     "Example": "string",
                     "ExampleTranslation": "string"
                   }
                 ]
               }
               """;
    }
}