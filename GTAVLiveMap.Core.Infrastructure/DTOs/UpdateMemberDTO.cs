using System;
using System.Collections.Generic;

namespace GTAVLiveMap.Core.Infrastructure.DTOs
{
    public class UpdateMemberDTO
    {
        public Guid Id { get; set; }
        public IList<string> Scopes { get; set; }
    }
}
