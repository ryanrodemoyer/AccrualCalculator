using System;
using System.Linq;
using AppName.Web.Models;
using AppName.Web.Services;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AppTests
{
    public class TestMigration : AppDataMigration
    {
        public TestMigration()
        {
            MigrationId = Guid.Parse("A4947A01-EABE-440D-B123-9903B0A5C7E0");
        }
    }
    
    [TestFixture]
    public class DefaultMigrationDiscoverService_Tests
    {
        [Test]
        public void FindAllMigrationsInGivenAssembly()
        {
            var discover = new DefaultMigrationDiscoverService();
            var results = discover.DiscoverMigrations(GetType().Assembly);
            
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(new TestMigration().MigrationId, results.First().MigrationId);
        }

        [Test]
        public void Test01()
        {
            var obj = new {value = Guid.NewGuid()};
            Console.WriteLine(obj.value);
            var json = JsonConvert.SerializeObject(obj);
            Console.WriteLine(json);
        }
    }
}