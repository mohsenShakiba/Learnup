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

        await using var stream = request.File.OpenReadStream();
        await using var coverStream = request.CoverImage?.OpenReadStream();

        var cmd = new UploadUserBook(
            stream,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            request.Title);

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

    [HttpGet("book/{name}", Name = "GetUserBookFile")]
    public async Task<IActionResult> GetFile(string name, CancellationToken cancellationToken)
    {
        var query = new GetUserBookFile(name);
        
        var file = await mediator.Send(query, cancellationToken);

        return file is null
            ? NotFound()
            : File(file.Content, file.ContentType, file.Id);
    }
}
