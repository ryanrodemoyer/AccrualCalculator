using AppName.Web.Models;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class EndingEnum : EnumerationGraphType<Ending>
    {
        public EndingEnum()
        {
            Name = "Ending";
            Description = "When should the accrual schedule end.";
            AddValue("CURRENTYEAR", "To the end of the current year.", 0);
            AddValue("PLUSONE", "To the end of the next year.", 1);
            AddValue("PLUSTWO", "End two years after the current year.", 2);
            AddValue("PLUSTHREE", "End three years after the current year.", 3);
        }
    }
}