namespace Learnup.Application.Prompts;

public static class ConversationTranslationPrompt
{
    public static string GetPrompt()
    {
        return """
               You are helping Farsi speakers learn English through short conversations.
               You are given the full conversation as an ordered list of sentences. Each sentence has an
               "Order" number and its English "Content".

               Translate the conversation sentence by sentence, but use the surrounding sentences as context
               so that pronouns, tense, tone and formality (informal تو vs formal شما) are consistent
               and natural across the whole conversation.

               For each sentence do two things:

               1. Translate it into natural, fluent Farsi, taking the whole conversation into account.
               2. Identify expressions, phrasal verbs or idioms in that sentence whose meaning an
                  English learner might misunderstand because the words do not mean what they
                  literally seem to mean in this context.
                  For example, in "it works for me", "work" does not mean "to labour" but rather
                  "to be suitable / acceptable". For each such expression, return the phrase exactly
                  as it appears, a short explanation of what it means in this context (in English),
                  and a Farsi translation of that contextual meaning.

               Only include expressions that are genuinely potentially confusing. If a sentence has
               none, return an empty array for it.

               Return one result per input sentence, keyed by the same "Order" value you were given.
               Do not add, drop, merge or reorder sentences.

               Return JSON only. No markdown, explanations, or additional text.
               Schema:
               {
                   "Items":
                   [
                       {
                           "Order": number,
                           "Translation": string,
                           "Expressions":
                           [
                               {
                                   "Phrase": string,
                                   "Meaning": string,
                                   "Translation": string
                               }
                           ]
                       }
                   ]
               }
               """;
    }
}
