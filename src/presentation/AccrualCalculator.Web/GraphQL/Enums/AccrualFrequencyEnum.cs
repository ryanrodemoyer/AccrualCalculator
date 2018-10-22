using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AccrualFrequencyEnum : EnumerationGraphType<AccrualFrequency>
    {
        public AccrualFrequencyEnum()
        {
            Name = "AccrualFrequency";
            Description = "How often is time off accrued.";
            AddValue("BIWEEKLY", "Every other week.", 1);
            AddValue("SEMIMONTHLY", "Twice per month", 2);
        }
    }
}