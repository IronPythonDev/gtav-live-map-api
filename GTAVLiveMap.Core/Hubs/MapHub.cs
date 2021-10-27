using GTAVLiveMap.Core.Infrastructure.Attributes;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Hubs
{
    [MapApiKeyValidateAttribute]
    public class MapHub : Hub
    {
        public async Task UpdateMarker(object position)
        {
            await Clients.All.SendAsync("OnUpdateMarker", position);
        }
    }
}
