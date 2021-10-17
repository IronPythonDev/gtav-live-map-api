using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure;
using GTAVLiveMap.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    public partial class MapController
    {
        [HttpPost("{id}/invites")]
        public async Task<IActionResult> CreateInvite(string id , [FromBody] CreateInviteDTO createInviteDTO)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound();

                var member = await MapMemberRepository.GetByMapIdAndUserIdAndScopes(map.Id, userId , new List<MapScopeNameEnum> { MapScopeNameEnum.CreateInvite });

                if (member == null) return NotFound();

                var scopes = string.Join(';' , createInviteDTO.Scopes.Select(p => p.ToString()));

                var invite = await InviteRepository.Add(new Domain.Entities.Invite
                {
                    MapId = map.Id,
                    Key = Generator.GetRandomString(6 , true),
                    Scopes = scopes
                });

                return Created($"map/{id}/invite/{invite.Id}" , invite);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
