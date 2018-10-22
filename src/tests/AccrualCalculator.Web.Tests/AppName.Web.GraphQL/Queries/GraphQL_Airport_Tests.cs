using AppName.Web.GraphQL;
using NUnit.Framework;

namespace AppTests.AppName.Web.GraphQL
{
    [TestFixture]
    public class GraphQL_Airport_Tests : QueryTestBase<TestQuerySchema>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Services.UseGraphQLForQueries();
        }

        [Test]
        public void GetAllAirportsField()
        {
            string query = @"
                query {
                    airports {
                        code
                        name
                    }
                }
            ";

            string expected = @"{
              airports: [
                  {
                    code: 'XYZ',
                    name: 'XYZ Regional Airport'
                  }
                ]
            }";

            AssertQuerySuccess(query, expected);
        }

        [Test]
        public void GetSingleAirportField()
        {
            string query = @"
                query {
                    airport(code: ""XYZ"") {
                        code
                        name
                    }
                }
            ";

            string expected = @"{
              airport: {
                    code: 'XYZ',
                    name: 'XYZ Regional Airport'
                  }
            }";

            AssertQuerySuccess(query, expected);
        }
    }
}