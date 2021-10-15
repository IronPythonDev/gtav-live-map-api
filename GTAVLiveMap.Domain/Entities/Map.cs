using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class Map : Identity<Guid>
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MaxMembers { get; set; }
        public int OwnerId { get; set; }
    }
}
