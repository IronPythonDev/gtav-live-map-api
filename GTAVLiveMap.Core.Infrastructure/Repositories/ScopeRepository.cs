using Dapper;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class ScopeRepository : BaseRepository, IScopeRepository
    {
        public ScopeRepository(
            DbContext dbContext) : base(dbContext) { }

        public Task<MapScope> Add(MapScope obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<MapScope>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<MapScope>(@"SELECT ""Id"", ""Name"" FROM public.""MapScopes"";" , new { })).ToList();
        }

        public Task<MapScope> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(MapScope obj)
        {
            throw new NotImplementedException();
        }
    }
}
