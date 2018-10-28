using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppName.Web.Models;
using GraphQL;
using Microsoft.CodeAnalysis;
using MongoDB.Driver;

namespace AppName.Web.Repositories
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<AppUser> Users => _database.GetCollection<AppUser>("users");

        public MongoUserRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public Task<AppUser> GetUserByUserIdAsync(string userId)
        {
            var result = (
                from u in Users.AsQueryable()
                where u.UserId == userId
                select u
            ).SingleOrDefault();
            
            return Task.FromResult(result);
        }

        public async Task<bool> UpsertUserAsync(AppUser user)
        {
            var filter = Builders<AppUser>.Filter.Eq(x => x.UserId, user.UserId);

            var update = 
                Builders<AppUser>.Update.Combine(
                Builders<AppUser>.Update.SetOnInsert(x => x.UserId, user.UserId),
                Builders<AppUser>.Update.Set(x => x.Name, user.Name),
                Builders<AppUser>.Update.Set(x => x.PictureUri, user.PictureUri),
                Builders<AppUser>.Update.SetOnInsert(x => x.DateCreated, user.DateCreated)
                );
            
            await Users.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<AppUser> {IsUpsert = true});

            return true;
        }

        public Task<List<AppUser>> GetAllUsersAsync()
        {
            var results = (
                from u in Users.AsQueryable()
                select u
            ).ToList();

            return Task.FromResult(results);
        }
    }
}
