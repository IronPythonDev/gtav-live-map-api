using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Core.Infrastructure.Services;
using GTAVLiveMap.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        public UserController(
            IUserRepository userRepository,
            IGoogleService googleService)
        {
            UserRepository = userRepository;
            GoogleService = googleService;
        }

        IUserRepository UserRepository { get; }
        IGoogleService GoogleService { get; }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int limit = int.MaxValue , [FromQuery] int offset = 0) =>
            Ok(await UserRepository.GetAll(limit, offset));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id) =>
            Ok(await UserRepository.GetById(id));

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
