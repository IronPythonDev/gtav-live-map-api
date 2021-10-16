using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class MapMember : Identity<Guid>
    {
        public string Roles { get; set; }
        public string Scopes { get; set; }
        public Guid MapId { get; set; }
        public int OwnerId { get; set; }
    }
}
