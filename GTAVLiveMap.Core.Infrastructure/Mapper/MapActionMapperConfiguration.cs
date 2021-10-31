using GTAVLiveMap.Core.Infrastructure.DTOs;
using GTAVLiveMap.Core.Infrastructure.Mapper.Base;
using GTAVLiveMap.Domain.Entities;

namespace GTAVLiveMap.Core.Infrastructure.Mapper
{
    public class MapActionMapperConfiguration : MapperConfigurationBase
    {
        public MapActionMapperConfiguration()
        {
            CreateMap<MapAction, MapActionDTO>();
            CreateMap<MapActionDTO, MapAction>();
        }
    }
}
