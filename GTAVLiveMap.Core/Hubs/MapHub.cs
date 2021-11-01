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
            IMapRepository mapRepository,
            IMapConfigRepository mapConfigRepository,
            IMapLabelRepository mapLabelRepository)
        {
            MapLabelService = mapLabelService;
            ConnectionRepository = connectionRepository;
            MapRepository = mapRepository;
            MapConfigRepository = mapConfigRepository;
            MapLabelRepository = mapLabelRepository;
        }

        IMapLabelService MapLabelService { get; }
        IMapLabelRepository MapLabelRepository { get; }
        IConnectionRepository ConnectionRepository { get; }
        IMapRepository MapRepository { get; }
        IMapConfigRepository MapConfigRepository { get; }

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

            await Clients.Caller.SendAsync("OnConnected");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var apiKey = Context.GetHttpContext().Request.Query["apiKey"];

            var map = await MapRepository.GetByApiKey(apiKey);

            if (map == null) return;

            var connection = await ConnectionRepository.GetByConnectionId(Context.ConnectionId);

            if (connection != null) ConnectionRepository.DeleteById(connection.Id);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task DeleteObject(string customId)
        {
            try
            {
                var connection = await ConnectionRepository.GetByConnectionId(Context.ConnectionId);

                if (connection == null) return;

                var label = await MapLabelRepository.GetByMapIdAndCustomId(connection.MapId, customId);

                if (label == null) return;

                MapLabelRepository.DeleteById(label.Id);

                await Clients.Clients((await ConnectionRepository.GetByMapId(connection.MapId)).Select(v => v.ConnectionId)).SendAsync("DeleteLabel", label);
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
        }

        public async Task CreateOrUpdateObject(MapLabelDTO mapLabelDTO)
        {
            try
            {
                var connection = await ConnectionRepository.GetByConnectionId(Context.ConnectionId);

                if (connection == null) return;

                var mapConfig = await MapConfigRepository.GetById(connection.MapId);

                var objectCount = await MapLabelRepository.GetCountByMapId(connection.MapId);

                if (objectCount >= mapConfig.MaxObjects)
                {
                    await Clients.Caller.SendAsync("Error", new
                    {
                        Type = 1,
                        Msg = "The limit of available Objects for your Map has been exceeded"
                    });

                    return;
                }

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
