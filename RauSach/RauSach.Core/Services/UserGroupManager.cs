using Microsoft.Extensions.Caching.Memory;
using RauSach.Core.FrameworkModels;
using RauSach.Core.Repositories;

namespace RauSach.Core.Services
{
    public class UserGroupManager : IUserGroupManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private const string UserRoleGroupCacheKey = "user-role-group";

        public UserGroupManager(IMemoryCache memoryCache,
            IUserRepository userRepository,
            IUserGroupRepository userGroupRepository)
        {
            _memoryCache = memoryCache;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
        }
        /// <summary>
        /// Checks if the user has the specified permissions.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="parameters">The permissions to check.</param>
        /// <returns>True if the user has any of the specified permissions, false otherwise.</returns>
        public bool HasPermission(string username, params string[] parameters)
        {
            // Get the user's role group from the cache or retrieve it from the repository if it's not cached.
            var roleGroup = _memoryCache.GetOrCreate($"{UserRoleGroupCacheKey}-{username}", m =>
            {
                // Retrieve the user from the repository.
                var user = _userRepository.GetByUsername(username);

                // Create a new anonymous object that contains the user's roles and groups.
                return new
                {
                    Roles = user?.CustomRoles ?? new List<string>(),
                    Groups = user?.Groups ?? new List<Guid>()
                };
            });

            // Check if the user has any of the specified roles.
            var hasRole = roleGroup.Roles.Intersect(parameters).Any();

            // If the user has any of the specified roles, return true.
            if (hasRole) return true;

            // Iterate over each group the user belongs to.
            foreach (var groupID in roleGroup.Groups)
            {
                // Get the group from the repository.
                var group = GetGroup(groupID);

                // Check if any of the group's roles match the specified permissions.
                var hasGroupRole = group?.Roles?.Intersect(parameters)?.Any() == true;

                // If the user has any of the group's roles, return true.
                if (hasGroupRole) return true;
            }

            // If the user doesn't have any of the specified permissions or group roles, return false.
            return false;
        }

        /// <summary>
        /// Retrieves all roles associated with a specific username from the cache.
        /// If the roles are not found in the cache, retrieves them from the user repository.
        /// Merges the roles from different groups and returns a distinct list of roles.
        /// </summary>
        /// <param name="username">The username of the user whose roles are to be retrieved.</param>
        /// <returns>A distinct list of roles associated with the user.</returns>
        public List<string> GetAllRoles(string username)
        {
            // Retrieve the role group associated with the user from the cache.
            // If the role group is not found in the cache, retrieve it from the user repository.
            var roleGroup = _memoryCache.GetOrCreate($"{UserRoleGroupCacheKey}-{username}", m =>
            {
                // Retrieve the user from the user repository.
                var user = _userRepository.GetByUsername(username);

                // Create a new anonymous object that contains the user's roles and groups.
                return new
                {
                    // If the user has custom roles, use them. Otherwise, create an empty list of roles.
                    Roles = user?.CustomRoles ?? new List<string>(),

                    // If the user belongs to any groups, use their IDs. Otherwise, create an empty list of IDs.
                    Groups = user?.Groups ?? new List<Guid>()
                };
            });

            // Iterate over each group the user belongs to.
            foreach (var groupID in roleGroup.Groups)
            {
                // Retrieve the group from the repository.
                var group = GetGroup(groupID);

                // Add the group's roles to the list of roles associated with the user.
                roleGroup.Roles.AddRange(group.Roles);
            }

            // Return a distinct list of roles associated with the user.
            return roleGroup.Roles.Distinct().ToList();
        }

        public UserGroup GetGroup(Guid groupId) => _memoryCache.GetOrCreate($"group-{groupId}", m => _userGroupRepository.Get(groupId));

        public User GetUser(string username) => _memoryCache.GetOrCreate($"user-{username}", m => _userRepository.GetByUsername(username));

        public async Task SetUserRolesAsync(string userId, List<string> roles)
        {
            if (roles?.Any() != true) return;

            var user = await _userRepository.GetByIdAsync(userId);
            await _userRepository.SetAsync(userId, nameof(User.CustomRoles), roles);
            _memoryCache.Remove($"{UserRoleGroupCacheKey}-{user.UserName}");
        }

        /// <summary>
        /// Sets the roles for a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="roles">The list of roles to set for the group.</param>
        public async Task SetGroupRolesAsync(Guid groupId, List<string> roles)
        {
            // Check if the list of roles is not null and contains any items.
            if (roles?.Any() != true) return;

            // Get the group from the repository.
            var group = await _userGroupRepository.GetAsync(groupId);

            // Set the roles for the group.
            await _userGroupRepository.SetAsync(groupId, nameof(User.Roles), roles);

            // Remove the group from the cache.
            _memoryCache.Remove($"group-{groupId}");
        }

        public async Task AddUserToGroupAsync(Guid groupId, string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user.Groups.Contains(groupId)) return;

            user.Groups.Add(groupId);

            await _userRepository.SetAsync(userId, nameof(User.Groups), user.Groups);
            _memoryCache.Remove($"{UserRoleGroupCacheKey}-{user.UserName}");
        }
    }
}
