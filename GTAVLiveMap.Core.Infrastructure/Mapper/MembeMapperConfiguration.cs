using GTAVLiveMap.Core.Infrastructure.DTOs;
using GTAVLiveMap.Core.Infrastructure.Mapper.Base;
using GTAVLiveMap.Domain.Entities;

namespace GTAVLiveMap.Core.Infrastructure.Mapper
{
    public class MembeMapperConfiguration : MapperConfigurationBase
    {
        public MembeMapperConfiguration()
        {
            CreateMap<MapMember, UpdateMemberDTO>();
            CreateMap<UpdateMemberDTO, MapMember>().AfterMap((src, dest) =>
            {
                dest.Scopes = string.Join(';', src.Scopes);
            });
        }
    }
}
