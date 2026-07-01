namespace Learnup.Application.Prompts;

public static class VocabTranslationPrompt
{
    public static string GetPrompt()
    {
        return """
               Translate the given English word into Farsi and return a valid JSON array with the following rules:

               * If there are multiple meanings, discard the rare and unlikely ones.
               * If the word has multiple meanings with the same type, comma separate the translations in the same object. 
               * If the word has multiple types e.g Noun, verb, return multiple objects in the array each having a different type and translations.
               * Include a short description only when necessary; otherwise use `null`.

               Return JSON only. No markdown, explanations, or additional text.
               Schema:
               {
                   "Translation": string,
                   "Description": string | null,
                   "Types":
                   [
                       {
                         "Translation": "string",
                         "Description": "string | null",
                         "Example": string,
                         "ExampleTranslation": string,
                         "Type": "Noun" | "Verb" | "Adjective" | "Adverb" 
                       }
                   ]
               }
               """;
    }
}
