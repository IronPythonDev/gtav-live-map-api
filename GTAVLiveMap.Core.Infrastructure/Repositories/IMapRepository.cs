using GTAVLiveMap.Domain.Entities;
using System;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IMapRepository : IRepository<Map , Guid>
    {
    }
}
