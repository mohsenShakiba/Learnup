using Learnup.Application.Features.Public.Books;
using Learnup.Application.Mediation;
using Learnup.Application.Responses.Public.Users;
using Learnup.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

public class UserBooksController(IMediator mediator) : BasePublicController
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

        var cmd = new UploadUserBook(
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
        var query = new GetUserBooks();

        var books = await mediator.Send(query, cancellationToken);

        return Ok(books);
    }

    [HttpPut("book/{id:int}", Name = "UpdateUserBookCurrentPage")]
    public async Task<IActionResult> UpdateCurrentPage(
        int id,
        [FromBody] UpdateUserBookCurrentPageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserBookCurrentPage(id, request.CurrentRef, request.Progress);

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
