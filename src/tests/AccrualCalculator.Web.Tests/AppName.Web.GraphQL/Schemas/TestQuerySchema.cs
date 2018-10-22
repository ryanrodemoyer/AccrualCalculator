using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class TestQuerySchema : Schema
    {
        public TestQuerySchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<AppQuery>();
        }
    }
}