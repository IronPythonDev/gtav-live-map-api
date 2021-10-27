using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IMapLabelRepository : IRepository<MapLabel, Guid>
    {
        public Task<IList<MapLabel>> GetByMapId(Guid mapId , int limit = int.MaxValue, int offset = 0);
        public Task<MapLabel> GetByMapIdAndCustomId(Guid mapId , string customId);
        public Task<int> GetCountByMapId(Guid mapId);
        public Task<MapLabel> UpdateByCustomId(MapLabel mapLabel);
    }
}
