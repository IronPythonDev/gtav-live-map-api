using Dapper;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class InviteRepository : BaseRepository, IInviteRepository
    {
        public InviteRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<Invite> Add(Invite obj)
        {
            return (await DbContext.QueryAsync<Invite>(@"INSERT INTO public.""Invites""(""Key"" , ""Scopes"" , ""MapId"") VALUES(@Key , @Scopes , @MapId);
                                                SELECT * FROM public.""Invites"" WHERE ""Key"" = @Key;", obj)).FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
            await DbContext.ExecuteAsync("DELETE FROM public.\"Invites\" WHERE \"Id\" = @Id;", new { Id = id });

        public async Task<IList<Invite>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<Invite>(
                $"SELECT * FROM public.\"Invites\" ORDER BY \"Id\" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<IList<Invite>> GetByMapId(Guid mapId, int limit = int.MaxValue, int offset = 0)
        {
            return (await DbContext.QueryAsync<Invite>(
                @"SELECT * FROM public.""Invites"" WHERE ""MapId"" = @MapId LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset, MapId = mapId })).ToList();
        }

        public async Task<Invite> GetById(Guid id)
        {
            return (await DbContext.QueryAsync<Invite>($"SELECT * FROM public.\"Invites\" WHERE \"Id\" = @Id;", new { Id = id })).FirstOrDefault();
        }

        public async void Update(Invite obj)
        {
            await DbContext.ExecuteAsync(@"UPDATE public.""Invites"" SET ""Scopes"" = @Scopes WHERE ""Invites"".""Id"" = @Id;", obj);
        }

        public async Task<Invite> GetByKey(string key)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Invite>(
                @"SELECT * FROM public.""Invites"" WHERE ""Key"" = @Key;",
                new { Key = key })).FirstOrDefault();
        }

        public async void DeleteByKey(string key) =>
            await DbContext.ExecuteAsync("DELETE FROM public.\"Invites\" WHERE \"Key\" = @Key;", new { Key = key });

        public async void UpdateMany(IList<Invite> invites)
        {
            var db = DbContext.GetConnection();

            int index = 0;

            var parameters = new DynamicParameters();
            var sql = new StringBuilder();

            foreach (var invite in invites)
            {
                sql.Append(@$"UPDATE public.""Invites"" SET ""Scopes"" = @Scopes{index} WHERE ""Invites"".""Id"" = @Id{index};");

                parameters.Add($"@Scopes{index}" , invite.Scopes); 
                parameters.Add($"@Id{index}" , invite.Id);

                index++;
            }

            await db.ExecuteAsync(sql.ToString(), parameters);
        }

        public async Task<int> GetCount()
        {
            return (await GetAll(int.MaxValue , 0)).Count;
        }
    }
}
