using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Core.Infrastructure.Services;
using GTAVLiveMap.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/users")]
    public partial class UserController : ControllerBase
    {

        public UserController(
            IUserRepository userRepository,
            IGoogleService googleService,
            IMapRepository mapRepository,
            IMapConfigRepository mapConfigRepository,
            IMapMemberRepository mapMemberRepository)
        {
            UserRepository = userRepository;
            GoogleService = googleService;
            MapRepository = mapRepository;
            MapConfigRepository = mapConfigRepository;
            MapMemberRepository = mapMemberRepository;
        }

        IUserRepository UserRepository { get; }
        IMapRepository MapRepository { get; }
        IMapMemberRepository MapMemberRepository { get; }
        IGoogleService GoogleService { get; }
        IMapConfigRepository MapConfigRepository { get; }

        [HttpGet]
        public async Task<IActionResult> GetUser() =>
            Ok(await UserRepository.GetById(int.Parse(HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value)));

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            try
            {
                if (string.IsNullOrEmpty(createUserDTO.JWT)) return UnprocessableEntity();

                var GoogleUser = await GoogleService.GetUserFromJWT(createUserDTO.JWT);

                if (GoogleUser == null) 
                    return BadRequest();

                if ((await UserRepository.GetByEmail(GoogleUser.Email)) != null)
                    return BadRequest();

                User user = await UserRepository.Add(new Domain.Entities.User { Email = GoogleUser.Email });

                return Created($"api/user/{user.Id}", user.Id);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
         
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            UserRepository.DeleteById(id);

            return Ok();
        }
    }
}
