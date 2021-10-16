using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class Invite : Identity<Guid>
    {
        public string Key { get; set; }
        public string Scopes { get; set; }
        public Guid MapId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
