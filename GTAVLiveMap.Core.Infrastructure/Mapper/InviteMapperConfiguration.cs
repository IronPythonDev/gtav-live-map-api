using GTAVLiveMap.Core.Infrastructure.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure.Mapper.Base;
using GTAVLiveMap.Domain.Entities;

namespace GTAVLiveMap.Core.Infrastructure.Mapper
{
    public class InviteMapperConfiguration : MapperConfigurationBase
    {
        public InviteMapperConfiguration()
        {
            CreateMap<Invite, UpdateInviteDTO>();
            CreateMap<UpdateInviteDTO, Invite>().AfterMap((src , dest) => 
            {
                dest.Scopes = string.Join(';' , src.Scopes);
            });
        }
    }
}
