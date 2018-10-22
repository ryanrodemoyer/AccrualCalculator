using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class RoleGraphType : ObjectGraphType<AppRole>
    {
        public RoleGraphType()
        {
            Name = "Role";

            Field<IdGraphType>(
                name: "roleId",
                description: "Role ID.");

            Field<StringGraphType>(
                name: "name",
                description: "Role Name.");
        }
    }
}