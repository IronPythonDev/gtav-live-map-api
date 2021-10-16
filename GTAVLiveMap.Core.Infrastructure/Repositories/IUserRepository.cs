using GTAVLiveMap.Domain.Entities;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<User , int>
    {
        Task<User> GetByEmail(string email);
    }
}
