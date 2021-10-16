using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IRepository<T , TIdentity>
    {
        Task<T> Add(T obj);
        void DeleteById(TIdentity id);
        void Update(T obj);
        Task<T> GetById(TIdentity id);
        Task<IList<T>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue);
    }
}
