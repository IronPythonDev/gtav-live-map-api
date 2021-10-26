using System;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public interface IUserUIService
    {
        Task<object> GetUserMenuByMemberIdAndMapId(int memberId, Guid mapId);
    }
}
