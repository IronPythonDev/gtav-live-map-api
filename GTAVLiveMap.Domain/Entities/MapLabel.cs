using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class MapLabel : Identity<Guid>
    {
        public Guid MapId { get; set; }
        public string Coordinates { get; set; }
        public string Vector3 { get; set; }
        public int Type { get; set; }
        public string PopupBody { get; set; }
        public string CustomId { get; set; }
        public string MetaData { get; set; }
    }
}
