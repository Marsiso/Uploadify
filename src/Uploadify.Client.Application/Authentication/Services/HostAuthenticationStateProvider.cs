using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Uploadify.Client.Domain.Authentication.Models;
using Uploadify.Client.Domain.Routing.Constants;

namespace Uploadify.Client.Application.Authentication.Services;

public class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _principalCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private readonly NavigationManager _navigation;
    private readonly HttpClient _client;
    private readonly ILogger<HostAuthenticationStateProvider> _logger;

    private DateTimeOffset _lastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

    public HostAuthenticationStateProvider(NavigationManager navigation, HttpClient client, ILogger<HostAuthenticationStateProvider> logger)
    {
        _navigation = navigation;
        _client = client;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(await GetUser(useCache: true));
    }

    public void SignIn(string customReturnUrl = null)
    {
        var returnUrl = customReturnUrl != null ? _navigation.ToAbsoluteUri(customReturnUrl).ToString() : null;
        var encodedReturnUrl = Uri.EscapeDataString(returnUrl ?? _navigation.Uri);
        var logInUrl = _navigation.ToAbsoluteUri($"{ApiRoutes.Account.LogIn}?returnUrl={encodedReturnUrl}");
        _navigation.NavigateTo(logInUrl.ToString(), true);
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = false)
    {
        var now = DateTimeOffset.Now;
        if (useCache && now < _lastCheck + _principalCacheRefreshInterval)
        {
            _logger.LogDebug("Taking user from cache ...");
            return _cachedPrincipal;
        }

        _logger.LogDebug("Fetching user ...");
        _cachedPrincipal = await FetchUser();
        _lastCheck = now;

        return _cachedPrincipal;
    }

    private async Task<ClaimsPrincipal> FetchUser()
    {
        UserInfo? userInfo = null;

        try
        {
            _logger.LogInformation(_client.BaseAddress.ToString());

            userInfo = await _client.GetFromJsonAsync<UserInfo>(ApiRoutes.UserInfo.BaseUserInfo);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Fetching user failed.");
        }

        if (userInfo is not { IsAuthenticated: true })
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var identity = new ClaimsIdentity(
            nameof(HostAuthenticationStateProvider),
            userInfo.NameClaimType,
            userInfo.RoleClaimType);

        if (userInfo.Claims == null)
        {
            return new ClaimsPrincipal(identity);
        }

        foreach (var claim in userInfo.Claims)
        {
            identity.AddClaim(new Claim(claim.Type, claim.Value));
        }

        return new ClaimsPrincipal(identity);
    }
}
