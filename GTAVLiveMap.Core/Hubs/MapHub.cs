using AutoMapper;
using GTAVLiveMap.Core.Infrastructure.Attributes;
using GTAVLiveMap.Core.Infrastructure.DTOs;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Core.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Hubs
{
    [MapApiKeyValidateAttribute]
    public class MapHub : Hub
    {
        public MapHub(
            IMapLabelService mapLabelService,
            IConnectionRepository connectionRepository,
            IMapRepository mapRepository)
        {
            MapLabelService = mapLabelService;
            ConnectionRepository = connectionRepository;
            MapRepository = mapRepository;
        }

        IMapLabelService MapLabelService { get; }
        IConnectionRepository ConnectionRepository { get; }
        IMapRepository MapRepository { get; }

        public override async Task OnConnectedAsync()
        {
            var apiKey = Context.GetHttpContext().Request.Query["apiKey"];

            var map = await MapRepository.GetByApiKey(apiKey);

            if (map == null)
            {
                Context.Abort();
                return;
            }

            await ConnectionRepository.Add(new Domain.Entities.Connection
            {
                ConnectionId = Context.ConnectionId,
                MapId = map.Id
            });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var apiKey = Context.GetHttpContext().Request.Query["apiKey"];

            var map = await MapRepository.GetByApiKey(apiKey);

            if (map == null)
            {
                Context.Abort();
                return;
            }

            var connection = await ConnectionRepository.GetByConnectionId(Context.ConnectionId);

            if (connection != null) ConnectionRepository.DeleteById(connection.Id);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateMarker(object position)
        {
            await Clients.All.SendAsync("OnUpdateMarker", position);
        }

        public async Task CreateOrUpdateObject(MapLabelDTO mapLabelDTO)
        {
            try
            {
                var connection = await ConnectionRepository.GetByConnectionId(Context.ConnectionId);

                if (connection == null) return;

                mapLabelDTO.MapId = connection.MapId;

                var label = await MapLabelService.CreateOrUpdateLabel(mapLabelDTO);

                await Clients.Clients((await ConnectionRepository.GetByMapId(connection.MapId)).Select(v => v.ConnectionId)).SendAsync("UpdateLabel", label);
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
        }
    }
}
