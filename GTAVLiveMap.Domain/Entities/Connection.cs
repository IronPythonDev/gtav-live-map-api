using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class Connection : Identity<Guid>
    {
        public string ConnectionId { get; set; }
        public Guid MapId { get; set; }
    }
}
