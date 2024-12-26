using MongoDB.Driver;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Repositories;

namespace RauSach.Database.Repositories
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
