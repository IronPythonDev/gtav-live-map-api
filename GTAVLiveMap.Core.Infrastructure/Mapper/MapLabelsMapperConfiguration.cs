using GTAVLiveMap.Core.Infrastructure.DTOs;
using GTAVLiveMap.Core.Infrastructure.Mapper.Base;
using GTAVLiveMap.Domain.Entities;
using System.Text.Json;

namespace GTAVLiveMap.Core.Infrastructure.Mapper
{
    public class MapLabelsMapperConfiguration : MapperConfigurationBase
    {
        public MapLabelsMapperConfiguration()
        {
            CreateMap<MapLabel, MapLabelDTO>().AfterMap((src, dest) =>
            {
                dest.Coordinates = JsonSerializer.Deserialize<object>(src.Coordinates);
                dest.MetaData = JsonSerializer.Deserialize<object>(src.MetaData);
            });

            CreateMap<MapLabelDTO, MapLabel>().AfterMap((src, dest) =>
            {
                var vector2Cords = GTAVConverter.GetLatLngFromVector2(src.Vector2);

                dest.Coordinates = JsonSerializer.Serialize(vector2Cords);
                dest.MetaData = JsonSerializer.Serialize(src.MetaData);
            });
        }
    }
}
