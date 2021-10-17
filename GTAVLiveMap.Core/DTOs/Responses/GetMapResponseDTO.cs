using GTAVLiveMap.Domain.Entities;
using System.Collections.Generic;

namespace GTAVLiveMap.Core.DTOs.Responses
{
    public class GetMapResponseDTO
    {
        public Map Map { get; set; }
        public IList<MapMember> Members { get; set; }
        public IList<Invite> Invites { get; set; }
    }
}
