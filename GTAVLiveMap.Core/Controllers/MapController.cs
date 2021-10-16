using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Core.Infrastructure.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    [Authorize]
    [Route("api/v1/map")]
    [ApiController]
    public partial class MapController : ControllerBase
    {
        public MapController(
            IMapRepository mapRepository,
            IInviteRepository inviteRepository,
            IUserRepository userRepository,
            IMapMemberRepository mapMemberRepository)
        {
            MapRepository = mapRepository;
            InviteRepository = inviteRepository;
            UserRepository = userRepository;
            MapMemberRepository = mapMemberRepository;
        }

        IMapRepository MapRepository { get; }
        IUserRepository UserRepository { get; }
        IInviteRepository InviteRepository { get; }
        IMapMemberRepository MapMemberRepository { get; }

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

                return Created(
                    $"api/v1/map/{map.Id}",
                    map);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError , 
                    new Generic() { StatusCode = 500 , Errors = new List<Error> { new Error { Title = "Internal Server Error", Description = "Internal Server Error" } } });
            }
        }
    }
}
