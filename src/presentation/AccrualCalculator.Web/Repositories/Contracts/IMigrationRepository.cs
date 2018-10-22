using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppName.Web.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AppName.Web.Repositories
{
    public interface IMigrationRepository
    {
        Task<List<Guid>> GetAllAppliedMigrationIdsAsync();

        Task AddMigrationComplete(AppDataMigration appDataMigration);
    }

    public class MongoMigrationRepository : IMigrationRepository
    {
        private readonly IMongoDatabase _database;

        private IMongoCollection<AppDataMigration> Migrations => _database.GetCollection<AppDataMigration>("migrations");

        public MongoMigrationRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public Task<List<Guid>> GetAllAppliedMigrationIdsAsync()
        {
            var results = (
                from m in Migrations.AsQueryable()
                select m.MigrationId
            ).ToList();

            return Task.FromResult(results);
        }

        public async Task AddMigrationComplete(AppDataMigration appDataMigration)
        {
            await Migrations.InsertOneAsync(appDataMigration);
        }
    }
}