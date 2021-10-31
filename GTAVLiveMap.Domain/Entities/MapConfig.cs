using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class MapConfig
    {
        public Guid MapId { get; set; }
        public int MaxInvites { get; set; } = 5;
        public int MaxMembers { get; set; } = 10;
        public int MaxLabels { get; set; } = 100;
    }
}
