using Dapper;
using GTAVLiveMap.Core.Contexts;
using GTAVLiveMap.Core.Entityes;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Repositories
{
    public class UserRepository : BaseRepository , IUserRepository
    {
        public UserRepository(DbContext dbContext): base(dbContext) { }

        public void Add(User obj) =>
            DbContext.Execute("INSERT INTO public.\"Users\"(\"Email\") VALUES(@Email);", new { Email = obj.Email });

        public void DeleteById(int id) =>
            DbContext.Execute("DELETE FROM public.\"Users\" WHERE \"Id\" = @Id;", new { Id = id });

        public async Task<IList<User>> GetAll(int limit = int.MaxValue, int offset = 0)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<User>(
                $"SELECT * FROM public.\"Users\" ORDER BY \"Id\" LIMIT @Limit OFFSET @Offset" , 
                new { Limit = limit , Offset = offset })).ToList();
        }

        public async Task<User> GetById(int id)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<User>($"SELECT * FROM public.\"Users\" WHERE \"Id\" = {id};")).FirstOrDefault();
        }

        public void Update(User obj)
        {
            throw new NotImplementedException();
        }
    }
}
