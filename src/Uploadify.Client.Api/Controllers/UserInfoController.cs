using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uploadify.Client.Api.Infrastructure.Controllers.Models;
using Uploadify.Client.Application.Authentication.Helpers;
using Uploadify.Client.Domain.Authentication.Models;

namespace Uploadify.Client.Api.Controllers;

public class UserInfoController : BaseApiController<UserInfoController>
{
    public UserInfoController(ILogger<UserInfoController> logger) : base(logger)
    {
    }

    [HttpGet("~/api/userinfo")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    public IActionResult GetUserInfo() => Ok(AuthenticationHelpers.ExtractUserInfo(User));
}
