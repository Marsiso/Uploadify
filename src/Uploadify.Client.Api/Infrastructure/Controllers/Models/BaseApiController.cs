using Microsoft.AspNetCore.Mvc;

namespace Uploadify.Client.Api.Infrastructure.Controllers.Models;

[ApiController]
public class BaseApiController<TController> : ControllerBase where TController : class
{
    protected readonly ILogger<TController> Logger;

    public BaseApiController(ILogger<TController> logger)
    {
        Logger = logger;
    }
}
