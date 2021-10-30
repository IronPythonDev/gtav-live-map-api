using System;

namespace GTAVLiveMap.Core.Infrastructure.DTOs
{
    public class MapLabelDTO
    {
        public Guid Id { get; set; }
        public Guid MapId { get; set; }
        public object Coordinates { get; set; }
        public Vector3 Vector2 { get; set; }
        public int Type { get; set; }
        public string PopupBody { get; set; }
        public string CustomId { get; set; }
        public object MetaData { get; set; } = null;
    }
}
