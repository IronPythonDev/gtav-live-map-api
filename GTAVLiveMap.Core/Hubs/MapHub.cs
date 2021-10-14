using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Hubs
{
    public class MapHub : Hub
    {
        public async Task UpdateMarker(object position)
        {
            await Clients.All.SendAsync("OnUpdateMarker" , position);
        }
    }
}
