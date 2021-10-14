using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        public UserController(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        IUserRepository UserRepository { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int limit = int.MaxValue , [FromQuery] int offset = 0) =>
            Ok(await UserRepository.GetAll(limit, offset));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id) =>
            Ok(await UserRepository.GetById(id));

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            UserRepository.Add(new Domain.Entities.User { Email = createUserDTO.Email });

            return Ok();
        }
         
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            UserRepository.DeleteById(id);

            return Ok();
        }
    }
}
