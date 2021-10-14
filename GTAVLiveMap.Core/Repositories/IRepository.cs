using GTAVLiveMap.Core.Entityes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Repositories
{
    public interface IRepository<T>
    {
        void Add(T obj);
        void DeleteById(int id);
        void Update(T obj);
        Task<T> GetById(int id);
        Task<IList<T>> GetAll(int limit = int.MaxValue , int offset = int.MaxValue);
    }
}
