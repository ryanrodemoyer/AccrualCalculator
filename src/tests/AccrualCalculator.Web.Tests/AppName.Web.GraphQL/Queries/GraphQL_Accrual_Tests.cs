using System.Collections.Generic;
using AppName.Web.GraphQL;
using GraphQL;
using NUnit.Framework;

namespace AppTests.AppName.Web.GraphQL
{
    [TestFixture]
    public class GraphQLQuery_Accrual_Tests : QueryTestBase<TestQuerySchema>
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Services.UseGraphQLForQueries();
        }

        [Test]
        public void GetAccrual_AccrualIdIsBlank()
        {
            string query = @"
                query($accrualId: ID!) {
                    accrual(accrualId: $accrualId) {
                        accrualId
                    }
                }
            ";

            string expected = @"{
                accrual: null
            }";

            var inputs = new Inputs(new Dictionary<string, object>
            {
                {
                    "accrualId", ""
                }
            });

            AssertQuerySuccess(query, expected, inputs);
        }

        [Test]
        public void GetAccrualField()
        {
            string query = @"
                query {
                    accrual(accrualId: ""505F5343-AB40-4CE8-B54E-6B60C38F4588"") {
                        accrualId name startingHours accrualRate maxHours accrualFrequency ending startingDate dayOfPayA dayOfPayB
                        actions {
                            actionDate accrualAction amount note dateCreated
                        }
                        user {
                            userId
                        }
                    }
                }
            ";

            string expected = @"{
                accrual: {
                    accrualId: '505f5343-ab40-4ce8-b54e-6b60c38f4588',
                    name: 'MockName',
                    startingHours: 11.0,
                    accrualRate: 7.0,
                    maxHours: 255.0,
                    accrualFrequency: 'SEMIMONTHLY',
                    ending: 'PLUSTHREE',
                    startingDate: '2018-10-06',
                    dayOfPayA: 7,
                    dayOfPayB: 21,
                    actions: [ {
                            actionDate: null,
                            accrualAction: 'CREATED',
                            amount: null,
                            note: null,
                            dateCreated: '2018-10-10T12:37:18.555'
                        }
                    ],
                    user: {
                        userId: 'unittest|784734738',
                    }
                }
            }";

            AssertQuerySuccess(query, expected);
        }
    }
}