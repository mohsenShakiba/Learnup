using Learnup.Application.Responses.Public.AudioBooks;
using Learnup.Domain.AggregateRoots.AudioBooks;

namespace Learnup.Application.Mappings;

public static class AudioBookMappings
{
    public static AudioBookResponse ToResponse(this AudioBooks audioBook)
    {
        return new AudioBookResponse(
            audioBook.Id,
            audioBook.Title,
            audioBook.Description,
            audioBook.Author,
            audioBook.Level,
            audioBook.Year,
            audioBook.WordCount,
            audioBook.Source,
            audioBook.CoverId,
            audioBook.Status.HasFlag(AudioBookStatus.Voiced));
    }

    public static AudioBookDetailResponse ToDetailResponse(this AudioBooks audioBook)
    {
        return new AudioBookDetailResponse(
            audioBook.Id,
            audioBook.Title,
            audioBook.Description,
            audioBook.Author,
            audioBook.Level,
            audioBook.Year,
            audioBook.WordCount,
            audioBook.Source,
            audioBook.Content,
            audioBook.CoverId,
            audioBook.VoiceId,
            audioBook.TimingJsonId,
            audioBook.Status.HasFlag(AudioBookStatus.Voiced),
            audioBook.Items
                .OrderBy(item => item.Order)
                .Select(item => item.ToResponse())
                .ToArray());
    }

    public static AudioBookItemResponse ToResponse(this AudioBookListItem item)
    {
        return new AudioBookItemResponse(
            item.Id,
            item.Sentence,
            item.Translation,
            item.Order,
            item.Expressions
                .Select(expression => expression.ToResponse())
                .ToArray());
    }

    public static AudioBookItemExpressionResponse ToResponse(this AudioBookListItemExpression expression)
    {
        return new AudioBookItemExpressionResponse(
            expression.Id,
            expression.Phrase,
            expression.Meaning,
            expression.Translation);
    }
}
