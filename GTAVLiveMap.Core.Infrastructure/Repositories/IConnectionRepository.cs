using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IConnectionRepository : IRepository<Connection, Guid>
    {
        Task<Connection> GetByConnectionId(string connectionId);
        Task<IList<Connection>> GetByMapId(Guid mapId);
    }
}
