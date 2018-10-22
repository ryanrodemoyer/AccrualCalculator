//using System;
//using System.Collections.Generic;
//using AppName.Web.Models;
//using MongoDB.Driver;
//
//namespace AppName.Web.Migrations
//{
//    public class AddBasicUsers : AppDataMigration
//    {
//        private readonly List<AppUser> _users = new List<AppUser>
//        {
//            new AppUser(1, "developer", "domainuser", true, "Nice", "Person", new[] {1, 2}  ),
//            new AppUser(2, "developer", "diffuser", true, "Naughty", "Person", new[] {1, 2} ),
//            //new AppUser(3, "yourdomain", "ad.user.name", true, "FirstName", "LastName"),
//        };
//        
//        public AddBasicUsers()
//        {
//            Name = "Add Basic Users";
//            MigrationId = Guid.Parse("B08A9E21-20A2-48E1-9CC6-E0E3B3CF2900");
//
//            Up = context =>
//            {
//                var usersCollection = context.Database.GetCollection<AppUser>("users");
//
//                usersCollection.InsertMany(_users);
//
//                return true;
//            };
//
//            Down = context =>
//            {
//                var usersCollection = context.Database.GetCollection<AppUser>("users");
//
//                foreach (var user in _users)
//                {
//                    usersCollection.DeleteOne(u => u.UserId == user.UserId);
//                }
//
//                return true;
//            };
//        }
//    }
//}