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
            var db = DbContext.GetConnection();

            obj.ApiKey = Generator.GetRandomString(30);

            return (await db.QueryAsync<Map>(@" INSERT INTO public.""Maps""(""Name"" , ""ApiKey"" , ""MaxMembers"" , ""OwnerId"") 
                                                VALUES(@Name , @ApiKey , @MaxMembers , @OwnerId);
                                                SELECT * FROM public.""Maps"" WHERE ""ApiKey"" = @ApiKey;", obj)).FirstOrDefault();
        }

        public void DeleteById(Guid id) =>
                DbContext.Execute(@"DELETE FROM public.""Maps"" WHERE ""Id"" = @Id;", new { Id = id });

        public async Task<IList<Map>> GetAll(int limit = int.MaxValue, int offset = int.MaxValue)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Map>(
                @"SELECT * FROM public.""Maps"" ORDER BY ""Id"" LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset })).ToList();
        }

        public async Task<Map> GetById(Guid id)
        {
            var db = DbContext.GetConnection();

            return (await db.QueryAsync<Map>(@"SELECT * FROM public.""Maps"" WHERE ""Id"" = @Id;", new { Id = id })).FirstOrDefault();
        }

        public void Update(Map obj)
        {
            throw new NotImplementedException();
        }
    }
}
