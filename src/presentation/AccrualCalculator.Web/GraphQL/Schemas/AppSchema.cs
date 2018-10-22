using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AppSchema : Schema
    {
        public AppSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<AppQuery>();
            Mutation = resolver.Resolve<AppMutation>();
        }
    }
}