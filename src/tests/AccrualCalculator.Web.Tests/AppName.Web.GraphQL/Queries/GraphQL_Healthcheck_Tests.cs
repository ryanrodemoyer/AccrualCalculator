using AppName.Web.GraphQL;
using NUnit.Framework;

namespace AppTests.AppName.Web.GraphQL
{
    [TestFixture]
    public class GraphQL_Healthcheck_Tests : QueryTestBase<TestQuerySchema>
    {
        [Test]
        public void HealthCheckField()
        {
            Services.UseGraphQLForQueries();

            string query = @"
                query {
                    healthcheck {
                        version
                        serverTimestamp
                    }
                }
            ";

            string expected = @"{
                healthcheck: {
                    version: '1.0.0',
                    serverTimestamp: '2018-10-10T08:08:08.027'
                }
            }";

            AssertQuerySuccess(query, expected);
        }
    }
}