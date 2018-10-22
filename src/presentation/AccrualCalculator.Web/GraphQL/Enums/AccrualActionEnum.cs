using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AccrualActionEnum : EnumerationGraphType<AccrualAction>
    {
        public AccrualActionEnum()
        {
            Name = "AccrualAction";
            Description = "";
            AddValue("CREATED", "Applied when an accrual chart is created.", 1);
            AddValue("ADJUSTMENT", "Applied when PTO is used against the accrual chart.", 2);
        }
    }
}