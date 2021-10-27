using Dapper;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class SessionKeyRepository : BaseRepository , ISessionKeyRepository
    {

        public SessionKeyRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<SessionKey> Add(SessionKey obj)
        {
            var db = DbContext.GetConnection();

            return (await DbContext.QueryAsync<SessionKey>(@"INSERT INTO public.""SessionKeys""
                                (""Key"" , ""CreatedIP"" , ""LastIP"" , ""UserAgent"" , ""OwnerId"") 
                                VALUES(@Key , @CreatedIP , @LastIP , @UserAgent , @OwnerId) 
                                RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(int id) =>
            await DbContext.ExecuteAsync("DELETE FROM public.\"SessionKeys\" WHERE \"Id\" = @Id;", new { Id = id });

        public async Task<IList<SessionKey>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            var db = DbContext.GetConnection();

            return (await DbContext.QueryAsync<SessionKey>(
                $"SELECT * FROM public.\"SessionKeys\" ORDER BY \"OwnerId\" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        [ObsoleteAttribute("This method is not implemented")]
        public Task<SessionKey> GetById(int id) =>
            throw new NotImplementedException();

        public async Task<SessionKey> GetByKey(string key)
        {
            var db = DbContext.GetConnection();

            return (await DbContext.QueryAsync<SessionKey>($"SELECT * FROM public.\"SessionKeys\" WHERE \"Key\" = @Key;" , new { Key = key })).FirstOrDefault();
        }

        public async Task<IList<SessionKey>> GetByOwnerId(int ownerId)
        {
            var db = DbContext.GetConnection();

            return (await DbContext.QueryAsync<SessionKey>($"SELECT * FROM public.\"SessionKeys\" WHERE \"OwnerId\" = '@OwnerId';", new { OwnerId = ownerId })).ToList();
        }

        public void Update(SessionKey obj)
        {
            throw new NotImplementedException();
        }
    }
}
