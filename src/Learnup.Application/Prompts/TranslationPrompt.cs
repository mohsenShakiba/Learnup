namespace Learnup.Infrastructure.Prompts;

public class TranslationPrompt
{
    public static string GetPrompt()
    {
        return """
               Translate the provided English word and English sentence to Farsi. 
               Return only valid JSON with this exact shape:
               { "wordTranslation": "...", "sentenceTranslation": "..." }.
               """;
    }
}