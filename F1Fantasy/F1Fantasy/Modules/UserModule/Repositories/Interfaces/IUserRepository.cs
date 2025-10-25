using F1Fantasy.Core.Common;

namespace F1Fantasy.Modules.UserModule.Repositories.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);

    Task<ApplicationUser?> GetUserByIdAsync(int id);

    Task<List<ApplicationUser>> FindUserByDisplayNameAsync(string name);

    Task<List<ApplicationUser>> GetAllUsersAsync();

    Task<(List<ApplicationUser> Users, int TotalCount)> SearchUsersAsync(string query, int skip, int take);
}