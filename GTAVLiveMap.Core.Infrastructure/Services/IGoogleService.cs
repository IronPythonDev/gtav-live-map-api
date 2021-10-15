using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public interface IGoogleService
    {
        Task<Payload> GetUserFromJWT(string idToken);
    }
}
