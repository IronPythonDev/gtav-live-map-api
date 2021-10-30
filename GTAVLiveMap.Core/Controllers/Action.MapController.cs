using GTAVLiveMap.Core.Infrastructure.Attributes;
using GTAVLiveMap.Core.Infrastructure.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        /// GET MAP ACTIONS
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        [HttpGet("{id}/actions")]
        [Scopes("ViewAction")]
        public async Task<IActionResult> GetActions(string id, [FromQuery] int limit = int.MaxValue, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var actions = await MapActionsRepository.GetByMapId(new Guid(id), limit, offset);

                Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
                Response.Headers.Add("X-Total-Count", $"{await MapActionsRepository.GetCountByMapId(new Guid(id))}");

                return Ok(actions);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Create Action for Map
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="CreateMapActionDTO">Action Custom Params</param>
        /// <returns></returns>
        [HttpPost("{id}/actions")]
        [Scopes("AddAction")]
        public async Task<IActionResult> CreateAction(string id, [FromBody] CreateMapActionDTO createInviteDTO)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var action = await MapActionsRepository.GetByMapIdAndName(map.Id, createInviteDTO.Name);

                if (action != null) return BadRequest("This action already exists");

                action = await MapActionsRepository.Add(new Domain.Entities.MapAction 
                { 
                    MapId = map.Id,
                    Name = createInviteDTO.Name,
                    Description = createInviteDTO.Description
                });

                return Created($"map/{id}/actions/{action.Id}", action);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Invoke Action for Map
        /// </summary>
        /// <param name="id">Map Id</param>
        /// <param name="CreateMapActionDTO">Action Custom Params</param>
        /// <returns></returns>
        [HttpPost("{id}/action/{name}")]
        [Scopes("EmitAction")]
        public async Task<IActionResult> InvokeAction(string id, string name , [FromBody] InvokeActionDTO invokeActionDTO)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var action = await MapActionsRepository.GetByMapIdAndName(map.Id, name);

                if (action == null) return BadRequest("Action not found");

                var mapConnections = await ConnectionRepository.GetByMapId(map.Id);

                await MapHubContext.Clients.Clients(mapConnections.Select(c => c.ConnectionId).ToList()).SendAsync(name, invokeActionDTO.Args);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}/action/{actionId}")]
        [Scopes("DeleteAction")]
        public async Task<IActionResult> DeleteAction(string id, string actionId)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                MapActionsRepository.DeleteById(new Guid(actionId));

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
