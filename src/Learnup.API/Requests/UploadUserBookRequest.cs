using Microsoft.AspNetCore.Http;

namespace Learnup.API.Requests;

public sealed class UploadUserBookRequest
{
    public string Title { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
    public IFormFile? CoverImage { get; set; }
}
