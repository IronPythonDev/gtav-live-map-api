using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    public partial class UserController
    {
        /// <summary>
        /// Get user maps
        /// </summary>
        /// <param name="limit">Limit</param>
        /// <param name="offset">Offset</param>
        /// <returns></returns>
        [HttpGet("maps")]
        public async Task<IActionResult> GetMaps([FromQuery] int limit = int.MaxValue, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value);

                var maps = await MapRepository.GetByUserId(userId);

                return Ok(maps);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
