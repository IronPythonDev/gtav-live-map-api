using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IInviteRepository : IRepository<Invite, Guid>
    {
        Task<IList<Invite>> GetByMapId(Guid id, int limit = int.MaxValue, int offset = 0);
        Task<Invite> GetByKey(string key);
        void DeleteByKey(string key);
        void UpdateMany(IList<Invite> invites);
    }
}
