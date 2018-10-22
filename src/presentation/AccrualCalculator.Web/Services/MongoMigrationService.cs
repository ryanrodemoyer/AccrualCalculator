using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Repositories;
using MongoDB.Driver;

namespace AppName.Web.Services
{
    public class MongoMigrationService : IMigrationService
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMigrationRepository _migrationRepository;
        private readonly IMigrationDiscoverService _migrationDiscoverService;
        private readonly IDotNetProvider _dotNetProvider;

        public MongoMigrationService(
            IMongoDatabase mongoDatabase,
            IMigrationRepository migrationRepository,
            IMigrationDiscoverService migrationDiscoverService,
            IDotNetProvider dotNetProvider)
        {
            _mongoDatabase = mongoDatabase;
            _migrationRepository = migrationRepository;
            _migrationDiscoverService = migrationDiscoverService;
            _dotNetProvider = dotNetProvider;
        }

        public async Task ApplyMigrationsAsync()
        {
            IEnumerable<AppDataMigration> migrations = _migrationDiscoverService.DiscoverMigrations(GetType().Assembly);
            List<Guid> ids = await _migrationRepository.GetAllAppliedMigrationIdsAsync();

            foreach (var migration in migrations)
            {
                if (ids.Contains(migration.MigrationId))
                {
                    continue;
                }

                migration.Timestamp = _dotNetProvider.DateTimeNow;

                var context = new MigrationContext
                {
                    Database = _mongoDatabase,
                    DotNetProvider = _dotNetProvider
                };
                
                bool upResult = migration.Up(context);
                if (upResult)
                {
                    await _migrationRepository.AddMigrationComplete(migration);
                }
                else
                {
                    bool downResult = migration.Down(context);
                    if (!downResult)
                    {
                        throw new InvalidOperationException("can you do anything correct?");
                    }
                }
            }
        }
    }

    public interface IMigrationDiscoverService
    {
        IEnumerable<AppDataMigration> DiscoverMigrations(Assembly assembly);
    }

    public class DefaultMigrationDiscoverService : IMigrationDiscoverService
    {
        public IEnumerable<AppDataMigration> DiscoverMigrations(Assembly assembly)
        {
            var types = assembly.GetTypes();

            var migrations = types
                .Where(t => t.BaseType == typeof(AppDataMigration))
                .Select(t => (AppDataMigration)Activator.CreateInstance(t));
            
            return migrations;
        }
    }
}