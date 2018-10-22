using System.Collections.Generic;
using System.Threading.Tasks;
using AppName.Web.Models;

namespace AppName.Web.Repositories
{
    public interface IRoleRepository
    {
        Task<AppRole> GetRoleByIdAsync(int roleId);
        Task<List<AppRole>> GetAllRolesAsync();
//        Task<List<AppRole>> GetRolesForUserAsync(int userId);
    }
}