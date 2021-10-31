using GTAVLiveMap.Domain.Entities;
using System;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IMapConfigRepository : IRepository<MapConfig, Guid>
    {
    }
}
