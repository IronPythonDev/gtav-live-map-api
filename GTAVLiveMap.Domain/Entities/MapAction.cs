using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class MapAction : Identity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public Guid MapId { get; set; }
    }
}
