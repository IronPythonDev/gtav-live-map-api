using AutoMapper;
using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.DTOs.Responses;
using GTAVLiveMap.Core.Infrastructure;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Core.Infrastructure.Responses;
using GTAVLiveMap.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    [Authorize]
    [Route("v1/maps")]
    [ApiController]
    public partial class MapController : ControllerBase
    {
        public MapController(
            IMapRepository mapRepository,
            IInviteRepository inviteRepository,
            IUserRepository userRepository,
            IMapMemberRepository mapMemberRepository,
            IMapper mapper)
        {
            MapRepository = mapRepository;
            InviteRepository = inviteRepository;
            UserRepository = userRepository;
            MapMemberRepository = mapMemberRepository;
            Mapper = mapper;
        }

        IMapRepository MapRepository { get; }
        IUserRepository UserRepository { get; }
        IInviteRepository InviteRepository { get; }
        IMapMemberRepository MapMemberRepository { get; }
        IMapper Mapper { get; }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMapById(string id)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var member = await MapMemberRepository.GetByMapAndUserId(map.Id, userId);

                if (member == null) return NotFound("Member not found");

                return Ok(map);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

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

                var inviteKey = await InviteRepository.Add(new Domain.Entities.Invite
                {
                    MapId = map.Id,
                    Key = Generator.GetRandomString(6, true),
                    Scopes = string.Join(';', Enum.GetNames(typeof(MapScopeNameEnum)))
                });

                await MapMemberRepository.Add(new Domain.Entities.MapMember
                {
                    Scopes = inviteKey.Scopes,
                    InviteKey = inviteKey.Key,
                    MapId = map.Id,
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
