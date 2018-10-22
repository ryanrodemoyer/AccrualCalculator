using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppName.Web.Models;

namespace AppName.Web.Repositories
{
//    public class InMemoryRoleRepository : IRoleRepository
//    {
//        private readonly List<AppRole> _roles = new List<AppRole>
//        {
//            new AppRole {Id=1, UserId = 1, Name = "admin"},
//            new AppRole {Id=2, UserId = 1, Name = "Access.Api"},
//            new AppRole {Id=3, UserId = 2, Name = "default"},
//        };
//
//        public Task<List<AppRole>> GetAllRolesAsync()
//        {
//            throw new System.NotImplementedException();
//        }
//
//        public Task<List<AppRole>> GetRolesForUserAsync(int userId)
//        {
//            var roles = _roles.Where(r => r.UserId == userId).ToList();
//
//            return Task.FromResult(roles);
//        }
//    }
}
