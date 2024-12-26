using RauSach.Core.FrameworkModels;

namespace RauSach.Core.Services
{
    public interface IUserGroupManager
    {
        List<string> GetAllRoles(string username);
        bool HasPermission(string userName, params string[] parameters);

        UserGroup GetGroup(Guid groupId);
        User GetUser(string username);
        Task SetUserRolesAsync(string userId, List<string> roles);

        Task SetGroupRolesAsync(Guid groupId, List<string> roles);
        Task AddUserToGroupAsync(Guid groupId, string userId);
    }
}
