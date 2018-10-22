using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AirportInputType : InputObjectGraphType
    {
        public AirportInputType()
        {
            Field<NonNullGraphType<IdGraphType>>("code", "The airport code.");
            Field<NonNullGraphType<StringGraphType>>("name", "The airport name.");
        }
    }
}