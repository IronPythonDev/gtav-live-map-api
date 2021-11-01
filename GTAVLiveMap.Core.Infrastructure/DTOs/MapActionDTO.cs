using System;

namespace GTAVLiveMap.Core.Infrastructure.DTOs
{
    public class MapActionDTO
    {
        public Guid Id { get; set; }
        public Guid MapId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = "";
        public string Source { get; set; } = "";
    }
}
