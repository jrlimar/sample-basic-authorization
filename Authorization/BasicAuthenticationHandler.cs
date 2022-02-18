using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace sample_basic_authorization.Authorization
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly PasswordAuthorization passwordAuthorization;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
                                          PasswordAuthorization passwordAuthorization)
            : base(options, logger, encoder, clock)
        {
            this.passwordAuthorization = passwordAuthorization;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            var headersAuthorizationKey = Request.Headers["Authorization"].ToString().Replace("Bearer", string.Empty).Replace("Basic", string.Empty);

            if (!passwordAuthorization.IsValid(headersAuthorizationKey))
            {
                return AuthenticateResult.Fail("Unauthorized -  Password Authorization");
            }

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "0") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var authentication = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(authentication));
        }
    }
}
