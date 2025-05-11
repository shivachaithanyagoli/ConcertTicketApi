// File: ConcertTicketApi.IntegrationTests/TestAuthHandler.cs

using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConcertTicketApi.IntegrationTests
{
#pragma warning disable 618  // suppress ISystemClock obsolete warning
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)    // use ISystemClock to match base ctor
            : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims   = new[] { new Claim(ClaimTypes.Name, "IntegrationTestUser") };
            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal= new ClaimsPrincipal(identity);
            var ticket   = new AuthenticationTicket(principal, SchemeName);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
#pragma warning restore 618
}
