//using System;
//using System.Collections.Generic;
//using AppName.Web.Models;
//using MongoDB.Driver;
//
//namespace AppName.Web.Migrations
//{
//    public class AddBasicRoles : AppDataMigration
//    {
//        private readonly List<AppRole> _roles = new List<AppRole>
//        {
//            new AppRole(1, "admin"),
//            new AppRole(2, "Access.Api"),
//            new AppRole(3, "default"),
//        };
//        
//        public AddBasicRoles()
//        {
//            Name = "Add Basic Roles for Seeding";
//            MigrationId = Guid.Parse("64762BDF-F59D-4B55-8A59-7C708E9EABD7");
//
//            Up = context =>
//            {
//                var rolesCollection = context.Database.GetCollection<AppRole>("roles");
//
//                rolesCollection.InsertMany(_roles);
//
//                return true;
//            };
//
//            Down = context =>
//            {
//                var rolesCollection = context.Database.GetCollection<AppRole>("roles");
//
//                foreach (var role in _roles)
//                {
//                    rolesCollection.DeleteOne(x => x.RoleId == role.RoleId);
//                }
//
//                return true;
//            };
//        }
//    }
//}