using AutoMapper;
using GTAVLiveMap.Core.Infrastructure.DTOs;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public class MapLabelService : IMapLabelService
    {
        public MapLabelService(
            IMapper mapper,
            IMapLabelRepository mapLabelRepository)
        {
            Mapper = mapper;
            MapLabelRepository = mapLabelRepository;
        }

        IMapper Mapper { get; }
        IMapLabelRepository MapLabelRepository { get; }

        public async Task<MapLabelDTO> CreateOrUpdateLabel(MapLabelDTO mapLabelDTO)
        {
            
            var label = await MapLabelRepository.GetByMapIdAndCustomId(mapLabelDTO.MapId , mapLabelDTO.CustomId);

            if (label == null) label = await MapLabelRepository.Add(Mapper.Map<MapLabel>(mapLabelDTO));
            else label = await MapLabelRepository.UpdateByCustomId(Mapper.Map<MapLabel>(mapLabelDTO));

            label.CustomId = await MapLabelRepository.GetCustomId(label);

            return Mapper.Map<MapLabelDTO>(label);
        }
    }
}
