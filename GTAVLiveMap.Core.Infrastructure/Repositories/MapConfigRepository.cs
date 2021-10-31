using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class MapConfigRepository : BaseRepository, IMapConfigRepository
    {
        public MapConfigRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public async Task<MapConfig> Add(MapConfig obj)
        {
            return (await DbContext.QueryAsync<MapConfig>(@"INSERT INTO public.""MapConfigs""
                                (""MapId"" , ""MaxInvites"" , ""MaxMembers"" , ""MaxLabels"" , ""MaxActions"") 
                                VALUES(@MapId , @MaxInvites , @MaxMembers , @MaxLabels , @MaxActions) 
                                RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
                await DbContext.ExecuteAsync(@"DELETE FROM public.""MapConfigs"" WHERE ""MapId"" = @MapId;", new { MapId = id });

        public async Task<IList<MapConfig>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<MapConfig>(
                @"SELECT * FROM public.""MapConfigs"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<MapConfig> GetById(Guid id)
        {
            return (await DbContext.QueryAsync<MapConfig>(@"SELECT * FROM public.""MapConfigs"" WHERE ""MapId"" = @MapId;", new { MapId = id }))
                .FirstOrDefault();
        }

        public async void Update(MapConfig obj)
        {
            await DbContext.ExecuteAsync(@"UPDATE public.""MapConfigs""
                                           SET ""MaxInvites"" = @MaxInvites, ""MaxMembers"" = @MaxMembers, ""MaxLabels"" = @MaxLabels , ""MaxActions"" = @MaxActions
                                           WHERE ""MapId"" = @MapId; ", obj);
        }
    }
}
