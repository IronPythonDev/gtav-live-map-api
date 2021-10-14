using GTAVLiveMap.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface ISessionKeyRepository : IRepository<SessionKey>
    {
        Task<IList<SessionKey>> GetByOwnerId(int ownerId);
        Task<SessionKey> GetByKey(string key);
    }
}
