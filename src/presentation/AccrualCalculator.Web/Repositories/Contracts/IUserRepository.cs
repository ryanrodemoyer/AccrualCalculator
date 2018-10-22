using System.Collections.Generic;
using System.Threading.Tasks;
using AppName.Web.Models;

namespace AppName.Web.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByUserIdAsync(string userId);
        Task<bool> UpsertUserAsync(AppUser user);
        Task<List<AppUser>> GetAllUsersAsync();
    }
}