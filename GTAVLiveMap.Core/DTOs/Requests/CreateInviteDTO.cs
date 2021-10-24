using GTAVLiveMap.Domain.Enums;
using System.Collections.Generic;

namespace GTAVLiveMap.Core.DTOs.Requests
{
    public class CreateInviteDTO
    {
        public IList<string> Scopes { get; set; }
    }
}
