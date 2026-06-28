using Learnup.Application.Responses.Public.Users;
using Learnup.Domain.AggregateRoots.Users;

namespace Learnup.Application.Mappers;

public static class UserBookMapper
{
    public static UserBookResponse ToResponse(
        this UserBook book) =>
        new(
            book.Id,
            book.Ebook.Title,
            book.Ebook.Author,
            book.Ebook.FileName,
            book.Ebook.CoverId,
            book.CurrentRef,
            book.Progress,
            book.Ebook.UploadedAt);
}
