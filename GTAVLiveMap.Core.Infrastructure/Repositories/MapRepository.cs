using Dapper;
using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class MapRepository : BaseRepository, IMapRepository
    {
        public MapRepository(
            DbContext dbContext) : base(dbContext) { }

        public async Task<Map> Add(Map obj)
        {
            obj.ApiKey = Generator.GetRandomString(30);

            return (await DbContext.QueryAsync<Map>(@" INSERT INTO public.""Maps""(""Name"" , ""ApiKey"" , ""MaxMembers"" , ""OwnerId"") 
                                                VALUES(@Name , @ApiKey , @MaxMembers , @OwnerId)  
                                                RETURNING *;", obj)).FirstOrDefault();
        }

        public async void DeleteById(Guid id) =>
                await DbContext.ExecuteAsync(@"DELETE FROM public.""Maps"" WHERE ""Id"" = @Id;", new { Id = id });

        public async Task<IList<Map>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            return (await DbContext.QueryAsync<Map>(
                @"SELECT * FROM public.""Maps"" ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<Map> GetByApiKey(string key)
        {
            return (await DbContext.QueryAsync<Map>(@"SELECT * FROM public.""Maps"" WHERE ""ApiKey"" = @Key;", new { Key = key })).FirstOrDefault();
        }

        public async Task<Map> GetById(Guid id)
        {
            return (await DbContext.QueryAsync<Map>(@"SELECT * FROM public.""Maps"" WHERE ""Id"" = @Id;", new { Id = id })).FirstOrDefault();
        }

        public async Task<IList<Map>> GetByUserId(int id, int limit = int.MaxValue, int offset = 0)
        {
            return (await DbContext.QueryAsync<Map>(
                @"SELECT ""Maps"".* FROM public.""MapMembers""
                  JOIN public.""Maps"" ON ""Maps"".""Id"" = ""MapMembers"".""MapId""
                  WHERE ""MapMembers"".""OwnerId"" = @OwnerId
                  ORDER BY ""Maps"".""Id"" OFFSET @Offset LIMIT @Limit;",
                new { Limit = limit, Offset = offset, OwnerId = id })).ToList();
        }

        public void Update(Map obj)
        {
            throw new NotImplementedException();
        }
    }
}
