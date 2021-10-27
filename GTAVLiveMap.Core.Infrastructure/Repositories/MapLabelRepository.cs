using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class MapLabelRepository : BaseRepository ,  IMapLabelRepository
    {
        public MapLabelRepository(DbContext dbContext): base(dbContext)
        {

        }

        public async Task<MapLabel> Add(MapLabel obj)
        {
            return (await DbContext.QueryAsync<MapLabel>(@" INSERT INTO public.""MapLabels""(""Coordinates"" , ""Type"" , ""PopupBody"" , ""MapId"" , ""MetaData"" , ""CustomId"") 
                                                VALUES(@Coordinates , @Type , @PopupBody , @MapId , @MetaData , @CustomId)
                                                RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
                await DbContext.ExecuteAsync(@"DELETE FROM public.""MapLabels"" WHERE ""Id"" = @Id;", new { Id = id });

        public async Task<IList<MapLabel>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<MapLabel>(
                @"SELECT * FROM public.""MapLabels"" ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<MapLabel> GetById(Guid id) =>
            (await DbContext.QueryAsync<MapLabel>(@"SELECT * FROM public.""MapLabels"" WHERE ""Id"" = @Id;", new { Id = id })).FirstOrDefault();

        public async Task<IList<MapLabel>> GetByMapId(Guid mapId, int limit = int.MaxValue, int offset = 0)
        {
            return (await DbContext.QueryAsync<MapLabel>(
                @"SELECT * FROM public.""MapLabels"" WHERE ""MapId"" = @MapId ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { MapId = mapId , Limit = limit, Offset = offset })).ToList();
        }

        public async Task<MapLabel> GetByMapIdAndCustomId(Guid mapId, string customId) =>
                    (await DbContext.QueryAsync<MapLabel>(@"SELECT * FROM public.""MapLabels"" WHERE ""MapId"" = @MapId AND ""CustomId"" = @CustomId;", new { MapId = mapId , CustomId = customId })).FirstOrDefault();

        public async Task<int> GetCountByMapId(Guid mapId) =>
            (await GetByMapId(mapId)).Count;

        public void Update(MapLabel obj)
        {
            throw new NotImplementedException();
        }

        public async Task<MapLabel> UpdateByCustomId(MapLabel mapLabel)
        {
            return (await DbContext.QueryAsync<MapLabel>(@" UPDATE public.""MapLabels""
	                                                        SET ""Coordinates"" = @Coordinates , ""Type"" = @Type, ""PopupBody"" = @PopupBody, ""MetaData""=@MetaData
	                                                        WHERE ""CustomId"" = @CustomId
	                                                        RETURNING *;", mapLabel)).FirstOrDefault();
        }
    }
}
