using AppName.Web.GraphQL;
using NUnit.Framework;

namespace AppTests.AppName.Web.GraphQL
{
    [TestFixture]
    public class GraphQL_Users_Tests : QueryTestBase<TestQuerySchema>
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Services.UseGraphQLForQueries();
        }

        [Test]
        public void AllUsersField()
        {
            string query = @"
                query {
                    users {
                        userId dateCreated
                    }
                }
            ";
            
//            new AppUser("unittest|784734738", new DateTime(2018, 10, 22, 18, 6, 6, 565)

            string expected = @"{
              users: [{
                userId: 'unittest|784734738',
                dateCreated: '2018-10-22T18:06:06.565'
              }]
            }";

            AssertQuerySuccess(query, expected);
        }

//        [Test]
//        public void GetSpecificUserField()
//        {
//            string query = @"
//                query {
//                    user(userId: ""unittest|784734738"") {
//                        userId dateCreated
//                    }
//                }
//            ";
//
//            string expected = @"{
//              user: {
//                userId: 'unittest|784734738',
//              }
//            }";
//
//            AssertQuerySuccess(query, expected);
//        }
    }
}