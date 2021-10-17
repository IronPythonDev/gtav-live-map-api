using Dapper;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using GTAVLiveMap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class MapMemberRepository : BaseRepository, IMapMemberRepository
    {
        public MapMemberRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public async Task<MapMember> Add(MapMember obj)
        {
            var db = DbContext.GetConnection();

            await db.ExecuteAsync(@"INSERT INTO public.""MapMembers""
                                    (""MapId"", ""OwnerId"", ""Scopes"" , ""InviteKey"")
                                    VALUES(@MapId , @OwnerId , @Scopes , @InviteKey);", obj);

            return null;
        }

        public void DeleteById(Guid id) =>
                DbContext.Execute("DELETE FROM public.\"MapMembers\" WHERE \"Id\" = @Id;", new { Id = id });

        public async Task<IList<MapMember>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<MapMember>(
                $"SELECT * FROM public.\"MapMembers\" ORDER BY \"Id\" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<MapMember> GetById(Guid id)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<MapMember>($"SELECT * FROM public.\"MapMembers\" WHERE \"Id\" = @Id;", new { Id = id }))
                .FirstOrDefault();
        }
        //
        public async Task<MapMember> GetByMapAndUserId(Guid mapId, int userId)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<MapMember>(@"SELECT * FROM public.""MapMembers"" 
                                                     WHERE ""MapId"" = @MapId AND ""OwnerId"" = @OwnerId;", 
                                                     new { MapId = mapId , OwnerId = userId})).FirstOrDefault();
        }

        public async Task<MapMember> GetByMapIdAndUserIdAndScopes(Guid mapId, int userId, IList<MapScopeNameEnum> scopes)
        {
            var db = DbContext.GetConnection();

            var _scopes = string.Join(';', scopes.Select(p => p.ToString()));

            return (await db.QueryAsync<MapMember>(@"SELECT * FROM public.""MapMembers"" 
                                                     WHERE ""MapId"" = @MapId AND ""OwnerId"" = @OwnerId AND ""Scopes"" LIKE '%' || @Scopes || '%';",
                                                     new { MapId = mapId, OwnerId = userId , Scopes = _scopes })).FirstOrDefault();
        }

        public async Task<IList<MapMember>> GetByMapId(Guid id, int limit = int.MaxValue, int offset = int.MaxValue)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<MapMember>(
                $"SELECT * FROM public.\"MapMembers\" WHERE \"Id\" = @Id ORDER BY \"Id\" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset , Id = id})).ToList();
        }

        public async Task<IList<MapMember>> GetByUserId(int id, int limit = int.MaxValue, int offset = int.MaxValue)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<MapMember>(
                @"SELECT * FROM public.""MapMembers"" WHERE ""OwnerId"" = @OwnerId ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset , OwnerId = id})).ToList();
        }

        public async void Update(MapMember obj)
        {
            var db = DbContext.GetConnection();

            await db.ExecuteAsync(@"UPDATE public.""MapMembers""
                                    SET ""Scopes"" = @Scopes
                                    WHERE ""Id"" = @Id; ", obj);
        }
    }
}
