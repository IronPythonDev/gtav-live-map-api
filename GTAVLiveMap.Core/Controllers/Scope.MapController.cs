using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    public partial class MapController
    {
        [HttpGet("{id}/scopes")]
        public async Task<IActionResult> GetScopes(string id)
        {
            try
            {
                return Ok(await ScopeRepository.GetAll(int.MaxValue, 0));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
