using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class HealthCheckGraphType : ObjectGraphType<Healthcheck>
    {
        public HealthCheckGraphType()
        {
            Name = "Healthcheck";

            Field(x => x.Version).Description("GraphQL API Version");
            Field<DateTimeGraphType>(
                name: "serverTimestamp",
                description: "Current time on server.");
        }
    }
}