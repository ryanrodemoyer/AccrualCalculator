using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AccrualRowGraphType : ObjectGraphType<AccrualRow>
    {
        public AccrualRowGraphType()
        {
            Field(x => x.RowId).Description("");
            Field(x => x.AccrualDate, nullable: true).Description("");
            Field(x => x.HoursUsed).Description("");
            Field(x => x.CurrentAccrual).Description("");

            Field(x => x.Actions, type: typeof(ListGraphType<AccrualActionRecordGraphType>)).Description("");
        }
    }
}