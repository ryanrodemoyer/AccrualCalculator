using GraphQL;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class TestMutationSchema : Schema
    {
        public TestMutationSchema(IDependencyResolver resolver) : base(resolver)
        {
            Mutation = resolver.Resolve<AppMutation>();
        }
    }
}