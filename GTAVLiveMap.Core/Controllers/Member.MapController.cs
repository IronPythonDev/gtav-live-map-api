using GTAVLiveMap.Core.Infrastructure.Attributes;
using GTAVLiveMap.Core.Infrastructure.DTOs;
using GTAVLiveMap.Domain.Entities;
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
        [HttpGet("{id}/member")]
        public async Task<IActionResult> GetMapMemberFromRequest(string id)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var member = await MapMemberRepository.GetByMapAndUserId(new Guid(id), userId);

                return Ok(member);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}/members")]
        [Scopes("ViewMembers")]
        public async Task<IActionResult> GetMapMembers(string id , [FromQuery] int limit = int.MaxValue, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var members = await MapMemberRepository.GetByMapId(new Guid(id), limit , offset);

                Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
                Response.Headers.Add("X-Total-Count", $"{await MapMemberRepository.GetCount()}");

                return Ok(members);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update members
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="invites"></param>
        /// <returns></returns>
        [HttpPut("{id}/members")]
        [Scopes("EditMember")]
        public async Task<IActionResult> UpdateMembers([FromRoute] string id, [FromBody] IList<UpdateMemberDTO> members)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                MapMemberRepository.UpdateMany(Mapper.Map<IList<MapMember>>(members));

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}/member")]
        public async Task<IActionResult> LeaveFromMap(string id)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var member = await MapMemberRepository.GetByMapAndUserId(map.Id, userId);

                if (member == null) return NotFound("Member not found");

                MapMemberRepository.DeleteById(member.Id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}/member/{memberId}")]
        [Scopes("DeleteMember")]
        public async Task<IActionResult> LeaveFromMap(string id , string memberId)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var member = await MapMemberRepository.GetByMapAndMemberId(map.Id , new Guid(memberId));

                if (member == null) return NotFound("Member not found");

                if (map.OwnerId == member.OwnerId) return BadRequest("You cannot remove the owner");

                if (member.OwnerId == userId) return BadRequest("You cannot delete yourself");

                MapMemberRepository.DeleteById(member.Id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
