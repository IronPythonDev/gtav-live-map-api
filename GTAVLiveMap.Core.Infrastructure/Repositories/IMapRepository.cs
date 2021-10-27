using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IMapRepository : IRepository<Map, Guid>
    {
        public Task<IList<Map>> GetByUserId(int id, int limit = int.MaxValue, int offset = 0);
        public Task<Map> GetByApiKey(string key);
    }
}
