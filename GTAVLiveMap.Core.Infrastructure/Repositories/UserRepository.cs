using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<User> Add(User obj)
        {
            return (await DbContext.QueryAsync<User>(@"INSERT INTO public.""Users""(""Email"" , ""Roles"" , ""TelegramID"") VALUES(@Email , @Roles , @TelegramID) RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(int id) =>
            await DbContext.ExecuteAsync("DELETE FROM public.\"Users\" WHERE \"Id\" = @Id;", new { Id = id });

        public async Task<IList<User>> GetAll(int limit = int.MaxValue, int offset = 0)
        {
            return (await DbContext.QueryAsync<User>(
                $"SELECT * FROM public.\"Users\" ORDER BY \"Id\" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<User> GetById(int id)
        {
            return (await DbContext.QueryAsync<User>(@"SELECT * FROM public.""Users"" WHERE ""Id"" = @Id;" , new { Id = id })).FirstOrDefault();
        }

        public async Task<User> GetByEmail(string email)
        {
            return (await DbContext.QueryAsync<User>($"SELECT * FROM public.\"Users\" WHERE \"Email\" = @Email;" , new { Email = email })).FirstOrDefault();
        }

        public void Update(User obj)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByTelegramID(string id)
        {
            return (await DbContext.QueryAsync<User>(@"SELECT * FROM public.""Users"" WHERE ""TelegramID"" = @TelegramID;", new { TelegramID = id })).FirstOrDefault();
        }

        public async Task<User> UpdateColumnById(int id , string name, string value)
        {
            return (await DbContext.QueryAsync<User>(@$"UPDATE public.""Users"" SET ""{name}"" = @ColumnValue  WHERE ""Id"" = @Id  RETURNING *;", new { Id = id , ColumnValue = value})).FirstOrDefault();
        }
    }
}
