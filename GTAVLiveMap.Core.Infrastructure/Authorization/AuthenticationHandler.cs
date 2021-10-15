using GTAVLiveMap.Core.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Authorization
{
    public class AuthenticationOptions : AuthenticationSchemeOptions { }

    public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
    {
        const string HEADER_KEY = "SessionKey";

        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserRepository userRepository,
            ISessionKeyRepository sessionKeyRepository) : base(options , logger , encoder , clock)
        {
            UserRepository = userRepository;
            SessionKeyRepository = sessionKeyRepository;
        }

        IUserRepository UserRepository { get; }
        ISessionKeyRepository SessionKeyRepository { get; }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HEADER_KEY))
                return AuthenticateResult.Fail("Unauthorized");

            string authorizationHeader = Request.Headers[HEADER_KEY];

            if (string.IsNullOrEmpty(authorizationHeader))
                return AuthenticateResult.NoResult();

            return await ValidateSessionKey(authorizationHeader);
        }

        private async Task<AuthenticateResult> ValidateSessionKey(string sessionKey)
        {
            var sessionKeyObject = await SessionKeyRepository.GetByKey(sessionKey);

            if (sessionKeyObject == null)
                return AuthenticateResult.Fail("Unauthorized");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier , $"{sessionKeyObject.OwnerId}")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
