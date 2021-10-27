using GTAVLiveMap.Core.Infrastructure.DTOs;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public interface IMapLabelService
    {
        Task<MapLabelDTO> CreateOrUpdateLabel(MapLabelDTO mapLabelDTO);
    }
}
