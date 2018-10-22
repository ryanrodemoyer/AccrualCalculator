using System;
using System.Collections;
using AppName.Web.Providers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace AppName.Web.Models
{
    public class AppDataMigration
    {
        public ObjectId _id { get; set; }

        public Guid MigrationId { get; set; }
        
        public string Name { get; set; }

        public DateTime Timestamp { get; set; }

        [BsonIgnore] public Func<MigrationContext, bool> Up { get; set; }

        [BsonIgnore] public Func<MigrationContext, bool> Down { get; set; }
    }

    public class MigrationContext
    {
        public IMongoDatabase Database { get; set; }
        public IDotNetProvider DotNetProvider { get; set; }
    }
}