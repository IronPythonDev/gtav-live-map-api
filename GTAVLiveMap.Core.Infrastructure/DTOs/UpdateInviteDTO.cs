using GTAVLiveMap.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GTAVLiveMap.Core.Infrastructure.DTOs.Requests
{
    public class UpdateInviteDTO
    {
        public Guid Id { get; set; }
        public IList<string> Scopes { get; set; }
    }
}
