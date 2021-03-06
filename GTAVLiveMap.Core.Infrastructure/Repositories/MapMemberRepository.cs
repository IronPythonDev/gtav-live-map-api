using Dapper;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using GTAVLiveMap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var members = await db.QueryAsync<MapMember, Map, MapMember>(@"INSERT INTO public.""MapMembers""
                                    (""MapId"", ""OwnerId"", ""Scopes"" , ""InviteKey"")
                                    VALUES(@MapId , @OwnerId , @Scopes , @InviteKey);
                                    SELECT * FROM public.""MapMembers"" 
                                    JOIN public.""Maps"" ON ""Maps"".""Id"" = @MapId
                                    WHERE ""MapMembers"".""MapId"" = @MapId AND ""MapMembers"".""OwnerId"" = @OwnerId;",
                                    (member, map) =>
                                    {
                                        member.Map = map;

                                        return member;
                                    }, obj);

            await db.CloseAsync();

            return members.FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
                await DbContext.ExecuteAsync("DELETE FROM public.\"MapMembers\" WHERE \"Id\" = @Id;", new { Id = id });

        public async Task<IList<MapMember>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<MapMember>(
                $"SELECT * FROM public.\"MapMembers\" ORDER BY \"Id\" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<MapMember> GetById(Guid id)
        {
            return (await DbContext.QueryAsync<MapMember>($"SELECT * FROM public.\"MapMembers\" WHERE \"Id\" = @Id;", new { Id = id }))
                .FirstOrDefault();
        }
        //
        public async Task<MapMember> GetByMapAndUserId(Guid mapId, int userId)
        {
            var db = DbContext.GetConnection();

            var result = (await db.QueryAsync<MapMember, Map, MapMember>(@"SELECT * FROM public.""MapMembers"" 
                                                     JOIN public.""Maps"" ON ""Maps"".""Id"" = @MapId
                                                     WHERE ""MapMembers"".""MapId"" = @MapId AND ""MapMembers"".""OwnerId"" = @OwnerId;",
                                                     (member, map) =>
                                                     {
                                                         member.Map = map;

                                                         return member;
                                                     },
                                                     new { MapId = mapId, OwnerId = userId })).FirstOrDefault();

            await db.CloseAsync();

            return result;
        }

        public async Task<MapMember> GetByMapIdAndUserIdAndScopes(Guid mapId, int userId, IList<MapScopeNameEnum> scopes)
        {
            var _scopes = string.Join(';', scopes.Select(p => p.ToString()));

            return (await DbContext.QueryAsync<MapMember>(@"SELECT * FROM public.""MapMembers"" 
                                                     WHERE ""MapId"" = @MapId AND ""OwnerId"" = @OwnerId AND ""Scopes"" LIKE '%' || @Scopes || '%';",
                                                     new { MapId = mapId, OwnerId = userId, Scopes = _scopes })).FirstOrDefault();
        }

        public async Task<IList<MapMember>> GetByMapId(Guid mapId, int limit = int.MaxValue, int offset = 0)
        {
            var db = DbContext.GetConnection();

            var result = (await db.QueryAsync<MapMember, User, MapMember>(
                @"SELECT * FROM public.""MapMembers"" 
                  JOIN public.""Users"" ON ""Users"".""Id"" = ""MapMembers"".""OwnerId"" 
                  WHERE ""MapId"" = @MapId 
                  ORDER BY ""MapMembers"".""Id"" LIMIT @Limit OFFSET @Offset",
                (member, user) =>
                {
                    member.User = user;

                    return member;
                },
                new { Limit = limit, Offset = offset, MapId = mapId })).ToList();

            await db.CloseAsync();

            return result;
        }

        public async Task<IList<MapMember>> GetByUserId(int id, int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<MapMember>(
                @"SELECT * FROM public.""MapMembers"" WHERE ""OwnerId"" = @OwnerId ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset, OwnerId = id })).ToList();
        }

        public async void Update(MapMember obj)
        {
            await DbContext.ExecuteAsync(@"UPDATE public.""MapMembers""
                                    SET ""Scopes"" = @Scopes
                                    WHERE ""Id"" = @Id; ", obj);
        }

        public async Task<int> GetCountByMapId(Guid mapId)
        {
            return (await GetByMapId(mapId , int.MaxValue, 0)).Count;
        }

        public async Task<MapMember> GetByMapAndMemberId(Guid mapId, Guid memberId)
        {
            var db = DbContext.GetConnection();

            var result = (await db.QueryAsync<MapMember, User, MapMember>(@"SELECT * FROM public.""MapMembers"" 
                                                     JOIN public.""Users"" ON ""Users"".""Id"" = ""MapMembers"".""OwnerId"" 
                                                     WHERE ""MapMembers"".""MapId"" = @MapId AND ""MapMembers"".""Id"" = @Id;",
                                                     (member, user) =>
                                                     {
                                                         member.User = user;

                                                         return member;
                                                     },
                                                     new { MapId = mapId, Id = memberId })).FirstOrDefault();

            await db.CloseAsync();

            return result;
        }

        public async void UpdateMany(IList<MapMember> members)
        {
            var db = DbContext.GetConnection();

            int index = 0;

            var parameters = new DynamicParameters();
            var sql = new StringBuilder();

            foreach (var member in members)
            {
                sql.Append(@$"UPDATE public.""MapMembers"" SET ""Scopes"" = @Scopes{index} WHERE ""MapMembers"".""Id"" = @Id{index};");

                parameters.Add($"@Scopes{index}", member.Scopes);
                parameters.Add($"@Id{index}", member.Id);

                index++;
            }

            await db.ExecuteAsync(sql.ToString(), parameters);
        }
    }
}
