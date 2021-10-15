using GTAVLiveMap.Core.DTOs.Requests;
using GTAVLiveMap.Core.Infrastructure;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using GTAVLiveMap.Core.Infrastructure.Services;
using GTAVLiveMap.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Controllers
{
    [Route("api/session-key")]
    [ApiController]
    public class SessionKeyController : ControllerBase
    {
        public SessionKeyController(
            ISessionKeyRepository sessionKeyRepository,
            IGoogleService googleService,
            IUserRepository userRepository)
        {
            SessionKeyRepository = sessionKeyRepository;
            GoogleService = googleService;
            UserRepository = userRepository;
        }

        ISessionKeyRepository SessionKeyRepository { get; }
        IGoogleService GoogleService { get; }
        IUserRepository UserRepository { get; }

        [HttpPost]
        public async Task<IActionResult> GenerateKeyAsync(GenerateSessionKeyDTO generateSessionKeyDTO)
        {
            var GoogleUser = await GoogleService.GetUserFromJWT(generateSessionKeyDTO.JWT);

            if (GoogleUser == null)
                return BadRequest();

            var user = await UserRepository.GetByEmail(GoogleUser.Email);

            if (user == null) 
                return BadRequest();

            var IP = HttpContext?.Connection?.RemoteIpAddress?.ToString();

            SessionKey sessionKey = await SessionKeyRepository.Add(new Domain.Entities.SessionKey
            {
                OwnerId = user.Id,
                Key = Generator.GetRandomString(30),
                LastIP = IP,
                CreatedIP = IP,
                UserAgent = generateSessionKeyDTO.UserAgent
            });

            return Created($"api/session-key/{sessionKey.Key}" , new { sessionKey = sessionKey.Key });
        }
    }
}
