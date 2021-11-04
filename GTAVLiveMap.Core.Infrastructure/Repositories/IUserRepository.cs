using GTAVLiveMap.Domain.Entities;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<User , int>
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByTelegramID(string id);
        Task<User> UpdateColumnById(int id , string name, string value);
    }
}
