using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uploadify.Server.Application.Authentication.Commands;
using Uploadify.Server.Application.Authentication.DataTransferObjects;
using Uploadify.Server.Application.Authentication.ViewModels;
using Uploadify.Server.Core.Application.Commands;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Requests.Models;

namespace Uploadify.Server.IdentityServer.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _manager;
    private readonly IMediator _mediator;
    private readonly ILogger<HomeController> _logger;

    public AccountController(SignInManager<User> manager, IMediator mediator, ILogger<HomeController> logger)
    {
        _manager = manager;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleLogin(string? returnUrl, LoginForm? form, CancellationToken cancellationToken)
    {
        returnUrl ??= Url.Content("~/");

        if (form == null)
        {
            return View("Login", new LoginViewModel() { ReturnUrl = returnUrl, Form = new LoginForm() });
        }

        var response = await _mediator.Send(form.Adapt<SignInPreProcessorCommand>(), cancellationToken);
        if (response.Status != Status.Ok)
        {
            return View("Login", new LoginViewModel() { ReturnUrl = returnUrl, Form = form, Errors = response.Failure?.Errors });
        }

        return LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");

        return View(new RegisterViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleRegister(string? returnUrl, RegisterForm? form, CancellationToken cancellationToken)
    {
        returnUrl ??= Url.Content("~/");

        if (form == null)
        {
            return View("Register", new RegisterViewModel { ReturnUrl = returnUrl, Form = new RegisterForm() });
        }

        var response = await _mediator.Send(form.Adapt<SignUpCommand>(), cancellationToken);
        if (response.Status != Status.Created)
        {
            return View("Register", new RegisterViewModel { ReturnUrl = returnUrl, Form = form, Errors = response.Failure?.Errors });
        }

        return View("Login", new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleLogout(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");

        await _manager.SignOutAsync();

        return LocalRedirect(returnUrl);
    }
}
