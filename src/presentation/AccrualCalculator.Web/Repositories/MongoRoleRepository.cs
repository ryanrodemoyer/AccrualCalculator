using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppName.Web.Models;
using MongoDB.Driver;

namespace AppName.Web.Repositories
{
    public class MongoRoleRepository : IRoleRepository
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<AppRole> Roles => _database.GetCollection<AppRole>("roles");

        public MongoRoleRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public Task<List<AppRole>> GetAllRolesAsync()
        {
            var roles = (
                from r in Roles.AsQueryable()
                select r).ToList();

            return Task.FromResult(roles);
        }

        public Task<AppRole> GetRoleByIdAsync(int roleId)
        {
            var role = (
                from r in Roles.AsQueryable()
                where r.RoleId == roleId
                select r
            ).FirstOrDefault();

            return Task.FromResult(role);
        }

//        public Task<List<AppRole>> GetRolesForUserAsync(int userId)
//        {
//            var roles = (
//                from r in Roles.AsQueryable()
////                where r.UserId == userId
//                select r).ToList();
//
//            return Task.FromResult(roles);
//        }
    }
}