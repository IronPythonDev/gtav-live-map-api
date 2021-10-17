using Dapper;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class InviteRepository : BaseRepository, IInviteRepository
    {
        public InviteRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<Invite> Add(Invite obj)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Invite>(@"INSERT INTO public.""Invites""(""Key"" , ""Scopes"" , ""MapId"") VALUES(@Key , @Scopes , @MapId);
                                                SELECT * FROM public.""Invites"" WHERE ""Key"" = @Key;", obj)).FirstOrDefault();
        }

        public void DeleteById(Guid id) =>
            DbContext.Execute("DELETE FROM public.\"Invites\" WHERE \"Id\" = @Id;", new { Id = id });

        public async Task<IList<Invite>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Invite>(
                $"SELECT * FROM public.\"Invites\" ORDER BY \"Id\" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<IList<Invite>> GetByMapId(Guid mapId, int limit = int.MaxValue, int offset = 0)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Invite>(
                @"SELECT * FROM public.""Invites"" WHERE ""MapId"" = @MapId ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset, MapId = mapId })).ToList();
        }

        public async Task<Invite> GetById(Guid id)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Invite>($"SELECT * FROM public.\"Invites\" WHERE \"Id\" = @Id;", new { Id = id })).FirstOrDefault();
        }

        public void Update(Invite obj)
        {
            throw new NotImplementedException();
        }

        public async Task<Invite> GetByKey(string key)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Invite>(
                @"SELECT * FROM public.""Invites"" WHERE ""Key"" = @Key;",
                new { Key = key })).FirstOrDefault();
        }

        public void DeleteByKey(string key) =>
            DbContext.Execute("DELETE FROM public.\"Invites\" WHERE \"Key\" = @Key;", new { Key = key });
    }
}
