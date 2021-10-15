using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    [Authorize]
    [Route("api/v1/map")]
    [ApiController]
    public class MapController : ControllerBase
    {
        public MapController(
            IMapRepository mapRepository)
        {
            MapRepository = mapRepository;
        }

        IMapRepository MapRepository { get; }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMap(CreateMapDTO createMapDTO)
        {
            try
            {
                var map = await MapRepository.Add(new Domain.Entities.Map
                {
                    Name = createMapDTO.Name,
                    MaxMembers = createMapDTO.MaxMembers,
                    OwnerId = createMapDTO.OwnerId
                });

                return Created($"api/v1/map/{map.Id}", map);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
