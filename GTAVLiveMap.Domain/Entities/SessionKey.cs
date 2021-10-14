using System;

namespace GTAVLiveMap.Domain.Entities
{
    public class SessionKey
    {
        public string Key { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAt { get; set; } = DateTime.Now;
        public string UserAgent { get; set; }
        public string CreatedIP { get; set; }
        public string LastIP { get; set; }
        public int OwnerId { get; set; }
    }
}
