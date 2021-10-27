using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public interface IMapApiKeyService
    {
        Task<bool> IsValid(string key);
    }
}
