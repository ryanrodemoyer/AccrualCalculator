﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppName.Web.DataStructures;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppName.Web.Models
{
    public class AppUser
    {
        public ObjectId _id { get; set; }
        
        public string UserId { get; set; }
        
        public string PictureUri { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public AppUser(string userId, DateTime dateCreated)
        {
            UserId = userId;
            DateCreated = dateCreated;
        }

//        public string Domain { get; private set; }
//        
//        public string UserName { get; private set; }
//
//        public bool IsActive { get; private set; }
//
//        public string FirstName { get; private set; }
//
//        public string LastName { get; private set; }
//        
//        public IEnumerable<int> RoleIds { get; private set; }
//        
//        [BsonIgnore]
//
//        public AppUser(string userId, string domain, string userName, bool isActive, string firstName, string lastName, IEnumerable<int> roleIds)
//        {
//            UserId = userId;
//            Domain = domain;
//            UserName = userName;
//            IsActive = isActive;
//            FirstName = firstName;
//            LastName = lastName;
//            RoleIds = roleIds;
//        }
    }
}