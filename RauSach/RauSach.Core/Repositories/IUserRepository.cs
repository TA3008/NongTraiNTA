using System.Collections.Generic;
using System.Threading.Tasks;
using RauSach.Core.FrameworkModels;

namespace RauSach.Core.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByIdAsync(string id);
        User GetByUsername(string username);
        Task<List<User>> FindAsync(FilterModel filter);
    }
}
