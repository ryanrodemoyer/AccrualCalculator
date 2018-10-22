using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AirportGraphType : ObjectGraphType<Airport>
    {
        public AirportGraphType()
        {
            Name = "Airport";

            Field<IdGraphType>("code", description: "Airport code.");
            Field(x => x.Name).Description("Airport full name.");
        }
    }
}