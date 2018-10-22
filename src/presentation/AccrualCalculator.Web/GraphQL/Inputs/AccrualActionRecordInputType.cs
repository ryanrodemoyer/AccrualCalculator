using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AccrualActionRecordInputType : InputObjectGraphType
    {
        public AccrualActionRecordInputType()
        {
            Field<NonNullGraphType<AccrualActionEnum>>("accrualAction", "");
            Field<NonNullGraphType<DateGraphType>>("actionDate", "");
            Field<NonNullGraphType<FloatGraphType>>("amount", "");
            Field<StringGraphType>("note", "");
        }
    }
}