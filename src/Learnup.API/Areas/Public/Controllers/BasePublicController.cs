using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnup.API.Areas.Public.Controllers;

[Area("Mobile")]
[ApiController]
[Authorize]
[Route("[area]/[controller]")]
public class BasePublicController : ControllerBase
{
}
