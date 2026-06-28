using Learnup.Application.Features.Public.Books;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Users;
using Learnup.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class BooksControllers(IMediator mediator) : BasePublicController
{
    [HttpPost(Name = "UploadUserBook")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserBookResponse>> Upload(
        [FromForm] UploadUserBookRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Book title is required.");
        }

        if (request.File is null || request.File.Length <= 0)
        {
            return BadRequest("Book file cannot be empty.");
        }

        await using var bookStream = request.File.OpenReadStream();

        await using var coverStream = request.CoverImage is { Length: > 0 } coverImage
            ? coverImage.OpenReadStream()
            : null;

        var cmd = new UploadBook(
            bookStream,
            request.File.ContentType,
            request.File.Length,
            request.Title,
            coverStream,
            request.CoverImage?.ContentType);

        await mediator.Send(cmd, cancellationToken);

        return NoContent();
    }

    [HttpGet(Name = "GetUserBooks")]
    public async Task<ActionResult<IReadOnlyList<UserBookResponse>>> Get(CancellationToken cancellationToken)
    {
        var query = new ListUserBooks();

        var books = await mediator.Send(query, cancellationToken);

        return Ok(books);
    }

    [HttpGet("{id:int}", Name = "GetUserBookById")]
    public async Task<ActionResult<UserBookResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var book = await mediator.Send(new GetUserBookById(id), cancellationToken);

        return book is null
            ? NotFound()
            : Ok(book);
    }

    [HttpPut("book/{id:int}", Name = "UpdateUserBookProgress")]
    public async Task<IActionResult> UpdateUserBookProgress(
        int id,
        [FromBody] UpdateUserBookCurrentPageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserBookCurrentPage(id, request.CurrentRef, request.Progress);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
