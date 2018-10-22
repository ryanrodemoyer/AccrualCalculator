using MongoDB.Bson;

namespace AppName.Web.Models
{
    public class AppRole
    {
        public ObjectId _id { get; set; }
        
        public int RoleId { get; set; }

        public string Name { get; set; }

        public AppRole(int roleId, string name)
        {
            RoleId = roleId;
            Name = name;
        }
    }
}