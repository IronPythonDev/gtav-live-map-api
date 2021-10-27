using GTAVLiveMap.Core.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public class MapApiKeyService : IMapApiKeyService
    {
        public MapApiKeyService(
            IMapRepository mapRepository)
        {
            MapRepository = mapRepository;
        }

        IMapRepository MapRepository { get; }

        public async Task<bool> IsValid(string key)
        {
            var map = await MapRepository.GetByApiKey(key);

            return map != null;
        }
    }
}
