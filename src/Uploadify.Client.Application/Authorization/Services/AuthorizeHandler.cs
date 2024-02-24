using System.Net;
using Uploadify.Client.Application.Authentication.Services;

namespace Uploadify.Client.Application.Authorization.Services;

public class AuthorizedHandler : DelegatingHandler
{
    private readonly HostAuthenticationStateProvider _authenticationStateProvider;

    public AuthorizedHandler(HostAuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

        HttpResponseMessage responseMessage;
        if (!authState.User.Identity.IsAuthenticated)
        {
            responseMessage = new(HttpStatusCode.Unauthorized);
        }
        else
        {
            responseMessage = await base.SendAsync(request, cancellationToken);
        }

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            _authenticationStateProvider.SignIn();
        }

        return responseMessage;
    }
}
