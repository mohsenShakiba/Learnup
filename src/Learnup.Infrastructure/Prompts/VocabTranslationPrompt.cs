namespace Learnup.Infrastructure.Prompts;

public static class VocabTranslationPrompt
{
    public static string GetPrompt()
    {
        return """
               Translate the following English word to Farsi.
               Provide one primary translation for the word.
               Provide a short description if needed.
               Provide the parent word if the word is a form of another word.
               Provide typed translation usages for each applicable part of speech.
               Only use these usage types: "Noun" and "Verb".
               For each typed usage, provide its translation, optional description, one English example, and the Farsi translation of that example.
               If the word is only a noun, return one Noun usage. If it is only a verb, return one Verb usage. If it can be both, return both.
               The response must be valid JSON and must follow this shape:
               {
                    "Translation": "string",
                    "Description": "string" | null,
                    "ParentWord": "string" | null,
                    "Transactions": [
                        {
                            "Type": "Noun" | "Verb",
                            "Translation": "string",
                            "Description": "string" | null,
                            "Example": "string",
                            "ExampleTranslation": "string"
                        }
                    ]
               }
               """;
    }
}
