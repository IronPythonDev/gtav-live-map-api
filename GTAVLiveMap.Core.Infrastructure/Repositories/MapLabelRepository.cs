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
            obj.CustomId = await GenerateCustomId(obj);

            return (await DbContext.QueryAsync<MapLabel>(@" 
                                                INSERT INTO public.""MapLabels""(""Coordinates"" , ""Type"" , ""PopupBody"" , ""MapId"" , ""MetaData"" , ""CustomId"") 
                                                VALUES(@Coordinates , @Type , @PopupBody , @MapId , @MetaData , @CustomId)
                                                RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
                await DbContext.ExecuteAsync(@"DELETE FROM public.""MapLabels"" WHERE ""Id"" = @Id;", new { Id = id });

        public async Task<IList<MapLabel>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            var labels = (await DbContext.QueryAsync<MapLabel>(
                @"SELECT * FROM public.""MapLabels"" ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();

            return labels.Select(async l =>
            {
                l.CustomId = await GetCustomId(l);
                return l;
            }).Select(t => t.Result).ToList();
        }

        public async Task<MapLabel> GetById(Guid id)
        {
            var label = (await DbContext.QueryAsync<MapLabel>(@"SELECT * FROM public.""MapLabels"" WHERE ""Id"" = @Id;", new { Id = id })).FirstOrDefault();

            label.CustomId = await GetCustomId(label);

            return label;
        }

        public async Task<IList<MapLabel>> GetByMapId(Guid mapId, int limit = int.MaxValue, int offset = 0)
        {
            var labels = (await DbContext.QueryAsync<MapLabel>(
                @"SELECT * FROM public.""MapLabels"" WHERE ""MapId"" = @MapId ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { MapId = mapId, Limit = limit, Offset = offset })).ToList();

            return labels.Select(async l =>
            {
                l.CustomId = await GetCustomId(l);
                return l;
            }).Select(t => t.Result).ToList();
        }

        public async Task<MapLabel> GetByMapIdAndCustomId(Guid mapId, string customId)
        {
            var label = (await DbContext.QueryAsync<MapLabel>(@"SELECT * FROM public.""MapLabels"" WHERE ""MapId"" = @MapId AND ""CustomId"" = @CustomId;",
                        new { MapId = mapId, CustomId = await GenerateCustomId(new MapLabel { MapId = mapId, CustomId = customId }) })).FirstOrDefault();

            label.CustomId = await GetCustomId(label);

            return label;
        }

        public async Task<int> GetCountByMapId(Guid mapId) =>
            (await GetByMapId(mapId)).Count;

        public void Update(MapLabel obj)
        {
            throw new NotImplementedException();
        }

        public async Task<MapLabel> UpdateByCustomId(MapLabel mapLabel)
        {
            mapLabel.CustomId = await GenerateCustomId(mapLabel);

            return (await DbContext.QueryAsync<MapLabel>(@" UPDATE public.""MapLabels""
	                                                        SET ""Coordinates"" = @Coordinates , ""Type"" = @Type, ""PopupBody"" = @PopupBody, ""MetaData""=@MetaData
	                                                        WHERE ""CustomId"" = @CustomId
	                                                        RETURNING *;", mapLabel)).FirstOrDefault();
        }

        public Task<string> GenerateCustomId(MapLabel mapLabel) =>
            Task.FromResult($"{mapLabel.MapId}:{mapLabel.CustomId}");

        public Task<string> GetCustomId(MapLabel mapLabel) =>
            Task.FromResult(mapLabel.CustomId.Split(':').Last());
    }
}
