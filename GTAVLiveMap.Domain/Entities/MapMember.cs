using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class MapMember : Identity<Guid>
    {
        public string Scopes { get; set; }
        public Guid MapId { get; set; }
        public int OwnerId { get; set; }
        public string InviteKey { get; set; }

        public User User { get; set; }
    }
}
