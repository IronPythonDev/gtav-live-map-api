using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public class GoogleService : IGoogleService
    {
        public GoogleService(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }

        public async Task<Payload> GetUserFromJWT(string jwt)
        {
            try
            {
                Payload payload = await ValidateAsync(jwt, new ValidationSettings
                {
                    Audience = new[] { Configuration["AuthProviders:Google:ClientId"] },
                    ExpirationTimeClockTolerance = TimeSpan.FromDays(1),
                    IssuedAtClockTolerance = TimeSpan.FromDays(1)
                });

                return payload;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
