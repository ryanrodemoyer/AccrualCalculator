using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AccrualInputType : InputObjectGraphType
    {
        public AccrualInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("name", "");
            Field<NonNullGraphType<FloatGraphType>>("startingHours", "");
            Field<NonNullGraphType<FloatGraphType>>("accrualRate", "");
            Field<NonNullGraphType<DateGraphType>>("startingDate", "");
            Field<NonNullGraphType<AccrualFrequencyEnum>>("accrualFrequency", "");
            Field<NonNullGraphType<EndingEnum>>("ending", "");

            Field<FloatGraphType>("hourlyRate", "");
            Field<IntGraphType>("dayOfPayA", "");
            Field<IntGraphType>("dayOfPayB", "");
            Field<FloatGraphType>("minHours", "");            
            Field<FloatGraphType>("maxHours", "");
            Field<BooleanGraphType>("isHeart", "");
            Field<BooleanGraphType>("isArchived", "");
        }
    }
}