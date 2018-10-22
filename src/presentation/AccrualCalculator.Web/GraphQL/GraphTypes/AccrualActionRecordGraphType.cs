using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AccrualActionRecordGraphType : ObjectGraphType<AccrualActionRecord>
    {
        public AccrualActionRecordGraphType()
        {
            Field(x => x.AccrualActionId, type: typeof(StringGraphType)).Description("");
            Field(x => x.AccrualAction, type: typeof(AccrualActionEnum)).Description("");
            Field(x => x.ActionDate, type: typeof(DateGraphType), nullable: true).Description("");
            Field(x => x.Amount, nullable: true).Description("");
            Field(x => x.Note, nullable: true).Description("");
            Field(x => x.DateCreated, type: typeof(DateTimeGraphType)).Description("");   
        }
    }
}