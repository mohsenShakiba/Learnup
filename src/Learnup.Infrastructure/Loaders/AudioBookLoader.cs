using Learnup.Application.Requests.Admin.AudioBooks;
using Learnup.Domain.AggregateRoots.AudioBooks;
using Learnup.Infrastructure.Persistence;

namespace Learnup.Infrastructure.ExternalService;

public class AudioBookLoader(LearnupDbContext dbContext)
{
    public async Task<int> LoadAsync(
        AudioBookImportRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var audioBook = new AudioBooks(
            request.Title.Trim(),
            Normalize(request.Description),
            Normalize(request.Author),
            Normalize(request.Level),
            Normalize(request.Year),
            Normalize(request.WordCount),
            Normalize(request.Source));

        foreach (var item in request.Items.Select((item, index) =>
                     new AudioBookListItem(
                         item.Sentence.Trim(),
                         Normalize(item.Translation),
                         index + 1)))
        {
            audioBook.AddItem(item);
        }

        if (request.Items.Count > 0 && request.Items.All(item => !string.IsNullOrWhiteSpace(item.Translation)))
        {
            audioBook.MarkAsTranslated();
        }

        dbContext.AudioBooks.Add(audioBook);
        await dbContext.SaveChangesAsync(cancellationToken);

        return audioBook.Id;
    }

    private static void Validate(AudioBookImportRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new InvalidOperationException("Audio book title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new InvalidOperationException("Audio book content is required.");
        }
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
