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
        [HttpGet("{id}/labels")]
        public async Task<IActionResult> GetMapLabels(string id, [FromQuery] int limit = int.MaxValue, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var labels = await MapLabelRepository.GetByMapId(map.Id, limit, offset);

                Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
                Response.Headers.Add("X-Total-Count", $"{await MapLabelRepository.GetCountByMapId(map.Id)}");

                var labelsDTOs = Mapper.Map<IList<MapLabelDTO>>(labels);

                return Ok(labelsDTOs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("{id}/labels")]
        public async Task<IActionResult> CreateMapLabel(string id, [FromBody] MapLabelDTO mapLabelDTO)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var labelDTO = Mapper.Map<MapLabel>(mapLabelDTO, c => c.AfterMap((src, dest) => dest.MapId = map.Id));

                var label = await MapLabelRepository.Add(labelDTO);

                return Ok(Mapper.Map<MapLabelDTO>(label));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}/label/{labelCustomId}")]
        public async Task<IActionResult> DeleteMapLabel(string id, string labelCustomId)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var map = await MapRepository.GetById(new Guid(id));

                if (map == null) return NotFound("Map not found");

                var label = await MapLabelRepository.GetByMapIdAndCustomId(map.Id , labelCustomId);

                if (label == null) return NotFound("Label not found");

                MapLabelRepository.DeleteById(label.Id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
