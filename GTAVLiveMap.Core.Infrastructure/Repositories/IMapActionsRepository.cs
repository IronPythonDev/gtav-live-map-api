using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IMapActionsRepository : IRepository<MapAction, Guid>
    {
        public Task<IList<MapAction>> GetByMapId(Guid id, int limit = int.MaxValue, int offset = 0);
        public Task<MapAction> GetByMapIdAndName(Guid id, string name);
        public Task<MapAction> GetByMapIdAndActionId(Guid id, Guid actionId);
        public Task<int> GetCountByMapId(Guid mapId);
    }
}
