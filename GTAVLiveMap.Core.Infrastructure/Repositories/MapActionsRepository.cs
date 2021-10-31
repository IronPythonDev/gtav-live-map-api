using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class MapActionsRepository : BaseRepository , IMapActionsRepository
    {
        public MapActionsRepository(DbContext dbContext) : base(dbContext)
        {

        }
        //MapActions
        public async Task<MapAction> Add(MapAction obj)
        {
            return (await DbContext.QueryAsync<MapAction>(@"INSERT INTO public.""MapActions""(""Name"" , ""MapId"" , ""Description"") 
                                                             VALUES(@Name , @MapId  , @Description) RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
                await DbContext.ExecuteAsync(@"DELETE FROM public.""MapActions"" WHERE ""Id"" = @Id;", new { Id = id });

        public async Task<IList<MapAction>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<MapAction>(
                @"SELECT * FROM public.""MapActions"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<MapAction> GetById(Guid id) =>
            (await DbContext.QueryAsync<MapAction>(@"SELECT * FROM public.""MapActions"" WHERE ""Id"" = @Id;", new { Id = id })).FirstOrDefault();

        public async Task<IList<MapAction>> GetByMapId(Guid id, int limit = int.MaxValue, int offset = 0)
        {
            return (await DbContext.QueryAsync<MapAction>(
                @"SELECT * FROM public.""MapActions"" WHERE ""MapId"" = @MapId LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset , MapId = id })).ToList();
        }

        public async Task<MapAction> GetByMapIdAndActionId(Guid id, Guid actionId)
        {
            return (await DbContext.QueryAsync<MapAction>(
                @"SELECT * FROM public.""MapActions"" WHERE ""MapId"" = @MapId AND ""Id"" = @ActionId;",
                new { ActionId = actionId , MapId = id })).FirstOrDefault();
        }

        public async Task<MapAction> GetByMapIdAndName(Guid id, string name)
        {
            return (await DbContext.QueryAsync<MapAction>(
                @"SELECT * FROM public.""MapActions"" WHERE ""MapId"" = @MapId AND ""Name"" = @Name;",
                new { Name = name , MapId = id })).FirstOrDefault();
        }

        public async Task<int> GetCountByMapId(Guid mapId) =>
            (await GetByMapId(mapId)).Count;

        public async void Update(MapAction obj)
        {
            await DbContext.QueryAsync<MapAction>(@"UPDATE public.""MapActions""
                                                    SET ""Name"" = @Name , ""Description"" = @Description
                                                    WHERE ""Id"" = @Id; ", obj);
        }
    }
}
