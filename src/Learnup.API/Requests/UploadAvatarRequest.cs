using Microsoft.AspNetCore.Http;

namespace Learnup.API.Requests;

public sealed class UploadAvatarRequest
{
    public IFormFile? File { get; set; }
}
