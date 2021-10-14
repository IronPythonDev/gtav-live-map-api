using GTAVLiveMap.Core.Infrastructure.Contexts;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public class BaseRepository
    {
        public BaseRepository(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public DbContext DbContext { get; }
    }
}
