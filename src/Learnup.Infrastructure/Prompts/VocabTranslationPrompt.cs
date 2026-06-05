namespace Learnup.Infrastructure.Prompts;

public static class VocabTranslationPrompt
{
    public static string GetPrompt()
    {
        return """
               Translate the following word to Farsi,
               Provide a single translation for the meaning,
               Provide a description of the word if needed,
               Provide a list of examples of usages of the word if needed,
               Provide a parent word if the word is a is a form of another word.
               The response should be in JSON format
               {
                    "Translation": "string",
                    "Description": "string" | null,
                    "Examples": ["string"] | [],
                    "ParentWord": "string" | null
               }
               
               Example:
               Said
               Return -> {"ParentId": "say", Translation: "Past tense of say"}}
               """;
    }
}