using AppName.Web.GraphQL;
using NUnit.Framework;

namespace AppTests.AppName.Web.GraphQL
{
    [TestFixture]
    public class GraphQL_Role_Tests : QueryTestBase<TestQuerySchema>
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Services.UseGraphQLForQueries();
        }

        [Test]
        public void RolesField()
        {
            string query = @"
                query {
                    roles {
                        roleId
                        name
                    }
                }
            ";

            string expected = @"{
              roles: [{
                roleId: '1',
                name: 'TestRole'
              }]
            }";

            AssertQuerySuccess(query, expected);
        }
    }
}