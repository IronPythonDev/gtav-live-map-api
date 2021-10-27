using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class ConnectionRepository : BaseRepository , IConnectionRepository
    {//Connections
        public ConnectionRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public async Task<Connection> Add(Connection obj)
        {
            return (await DbContext.QueryAsync<Connection>(@"INSERT INTO public.""Connections""(""ConnectionId"" , ""MapId"") 
                                                             VALUES(@ConnectionId , @MapId) RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
                await DbContext.ExecuteAsync(@"DELETE FROM public.""Connections"" WHERE ""Id"" = @Id;", new { Id = id });

        public async Task<IList<Connection>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<Connection>(
                @"SELECT * FROM public.""Connections"" ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<Connection> GetByConnectionId(string connectionId)
        {
            return (await DbContext.QueryAsync<Connection>(@"SELECT * FROM public.""Connections"" 
                                                             WHERE ""ConnectionId"" = @ConnectionId;", new { ConnectionId = connectionId }))
                                                            .FirstOrDefault();
        }

        public async Task<Connection> GetById(Guid id)
        {
            return (await DbContext.QueryAsync<Connection>(@"SELECT * FROM public.""Connections"" WHERE ""Id"" = @Id;", new { Id = id })).FirstOrDefault();
        }

        public async Task<IList<Connection>> GetByMapId(Guid mapId)
        {
            return (await DbContext.QueryAsync<Connection>(@"SELECT * FROM public.""Connections"" 
                                                             WHERE ""MapId"" = @MapId;", new { MapId = mapId })).ToList();
        }

        public void Update(Connection obj)
        {
            throw new NotImplementedException();
        }
    }
}
