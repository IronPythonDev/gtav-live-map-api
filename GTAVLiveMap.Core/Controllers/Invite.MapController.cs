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
        /// <summary>
        /// Get Map Invites
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns></returns>
        [HttpGet("{id}/invites")]
        public async Task<IActionResult> GetInvites([FromRoute] string id, [FromQuery] int limit = int.MaxValue, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var member = await MapMemberRepository.GetByMapAndUserId(map.Id, userId);

                if (member == null) return NotFound("Member not found");

                var invites = await InviteRepository.GetByMapId(map.Id, limit, offset);

                return Ok(invites);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Invite By Key
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="key">Invite Key</param>
        /// <returns></returns>
        [HttpGet("{id}/invites/{key}")]
        public async Task<IActionResult> GetInviteByKey([FromRoute] string id, [FromRoute] string key)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var member = await MapMemberRepository.GetByMapAndUserId(map.Id, userId);

                if (member == null) return NotFound("Member not found");

                var invite = await InviteRepository.GetByKey(key);

                if (invite == null) return NotFound("Invite not found");

                return Ok(invite);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create Invite for Map
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="createInviteDTO">Invite Custom Params</param>
        /// <returns></returns>
        [HttpPost("{id}/invites")]
        public async Task<IActionResult> CreateInvite(string id, [FromBody] CreateInviteDTO createInviteDTO)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var member = await MapMemberRepository.GetByMapIdAndUserIdAndScopes(map.Id, userId, new List<MapScopeNameEnum> { MapScopeNameEnum.CreateInvite });

                if (member == null) return NotFound("Member not found");

                var scopes = string.Join(';', createInviteDTO.Scopes.Select(p => p.ToString()));

                var invite = await InviteRepository.Add(new Domain.Entities.Invite
                {
                    MapId = map.Id,
                    Key = Generator.GetRandomString(6, true),
                    Scopes = scopes
                });

                return Created($"map/{id}/invite/{invite.Id}", invite);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Activate invite 
        /// </summary>
        /// <param name="inviteId">Invite Key</param>
        /// <returns></returns>
        [HttpPost("invites/{key}")]
        public async Task<IActionResult> CreateInvite([FromRoute] string key)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var invite = await InviteRepository.GetByKey(key);

                if (invite == null) return NotFound("Invite not found");

                var member = await MapMemberRepository.GetByMapAndUserId(invite.MapId, userId);

                if (member != null)
                {
                    member.Scopes = invite.Scopes;

                    MapMemberRepository.Update(member);

                    return NoContent();
                }

                await MapMemberRepository.Add(new Domain.Entities.MapMember
                {
                    OwnerId = userId,
                    MapId = invite.MapId,
                    Scopes = invite.Scopes,
                    InviteKey = invite.Key
                });

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Delete Invite By Key
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="key">Invite Key</param>
        /// <returns></returns>
        [HttpDelete("{id}/invites/{key}")]
        public async Task<IActionResult> DeleteInviteById([FromRoute] string id, [FromRoute] string key)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var member = await MapMemberRepository.GetByMapAndUserId(map.Id, userId);

                if (member == null) return NotFound("Member not found");

                InviteRepository.DeleteByKey(key);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
