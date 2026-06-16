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
               * Do not include rare, archaic, or highly unlikely parts of speech (e.g., "ticket" as a verb, "buy" as a noun).
               * For type you can use any of the following: Noun, Verb, Adjective, Adverb.
               
               Return JSON only. No markdown, explanations, or additional text.
               Schema:
               {
                 "Translation": "string",
                 "Description": "string | null",
                 "ParentWord": "string | null",
                 "Types": ["Noun" | "Verb" | "Adjective" | "Adverb"] 
               }
               """;
    }
}