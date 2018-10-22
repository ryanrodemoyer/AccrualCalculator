using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppName.Web.GraphQL;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Repositories;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace AppTests.AppName.Web.GraphQL
{
    public static class MockService
    {
        private static Dictionary<Type, object> _defaults = new Dictionary<Type, object>();

        static MockService()
        {
            RegisterAirportRepository();
            RegisterHttpContextAccessor();
            RegisterDotNetProvider();
            RegisterUsersRepository();
            RegisterRoleRepository();
            RegisterDashboardRepository();
        }

        public static T GetService<T>() where T : class
        {
            Type type = typeof(T);

            return ((Mock<T>) _defaults[type]).Object;
        }

        private static void RegisterRoleRepository()
        {
            var roles = new List<AppRole> {new AppRole(1, "TestRole")};

            var roleRepository = new Mock<IRoleRepository>();
            roleRepository.Setup(_ => _.GetAllRolesAsync()).Returns(
                Task.FromResult(roles)
            );
            roleRepository.Setup(_ => _.GetRoleByIdAsync(It.IsAny<int>())).Returns(
                (int roleId) => { return Task.FromResult(roles.FirstOrDefault(x => x.RoleId == roleId)); }
            );

            _defaults.Add(typeof(IRoleRepository), roleRepository);
        }

        private static void RegisterUsersRepository()
        {
            var users = new List<AppUser> {new AppUser("unittest|784734738", new DateTime(2018, 10, 23, 8, 14, 54, 187))};

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(_ => _.GetAllUsersAsync()).Returns(Task.FromResult(users));
            userRepository.Setup(_ => _.GetUserByUserIdAsync(It.IsAny<string>())).Returns(
                (string userId) => { return Task.FromResult(users.FirstOrDefault(x => x.UserId == userId)); }
            );

            _defaults.Add(typeof(IUserRepository), userRepository);
        }

        private static void RegisterDotNetProvider()
        {
            var dotNetProvider = new Mock<IDotNetProvider>();
            dotNetProvider.Setup(_ => _.NewGuid).Returns(Guid.Parse("9d2383fd-4f83-4982-a889-9b397170168e"));
            dotNetProvider.Setup(_ => _.DateTimeNow)
                .Returns(new DateTime(2018, 10, 5, 18, 1, 58, 999));

            _defaults.Add(typeof(IDotNetProvider), dotNetProvider);
        }

        private static void RegisterAirportRepository()
        {
            var airportRepository = new Mock<IAirportRepository>();
            var airports = new Dictionary<string, Airport>
            {
                {"XYZ", new Airport("XYZ", "XYZ Regional Airport")}
            };

            airportRepository.Setup(_ => _.GetAirports()).Returns(
                airports.Select(x => x.Value).ToList()
            );
            airportRepository.Setup(_ => _.GetAirport(It.IsAny<string>())).Returns(
                (string code) =>
                {
                    airports.TryGetValue(code, out var airport);
                    return airport;
                }
            );
            airportRepository.Setup(_ => _.AddAirport(It.IsAny<AirportInput>())).Callback((AirportInput input) =>
            {
                airports.Add(input.Code, new Airport(input.Code, input.Name));
            });

            _defaults.Add(typeof(IAirportRepository), airportRepository);
        }

        private static void RegisterHttpContextAccessor()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {new Claim(ClaimTypes.NameIdentifier, "unittest|784734738")}));

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            _defaults.Add(typeof(IHttpContextAccessor), httpContextAccessor);
        }

        private static void RegisterDashboardRepository()
        {
            var accruals = new Dictionary<Guid, Accrual>
            {
                {
                    Guid.Parse("15548dda-9447-425d-ad7a-c19ffb000621"), new Accrual(
                        accrualId: Guid.Parse("15548dda-9447-425d-ad7a-c19ffb000621"), name: "AccrualForAddAction", userId: "unittest|784734738", startingHours: 27.5, accrualRate: 7.66, maxHours: 255,
                        accrualFrequency: AccrualFrequency.SemiMonthly, dayOfPayA: 5, dayOfPayB: 22, startingDate: new DateTime(2018, 12, 1), ending: Ending.CurrentYear, isHeart: false,
                        isArchived: false, minHours: 30.5, lastModified: new DateTime(2018, 11, 30, 23, 59, 59, 999), hourlyRate: 15.83m,
                        actions: new List<AccrualActionRecord>
                        {
                            new AccrualActionRecord("5ecee8e4-36ca-4a4b-bc2e-c1aebc5c6b3f", AccrualAction.Created, null,
                                null, null, new DateTime(2018, 11, 30))
                        })
                },
                {
                    Guid.Parse("1851350b-0ce1-432b-a453-f6ffbe0bfc3c"),
                    new Accrual(
                        accrualId: Guid.Parse("1851350b-0ce1-432b-a453-f6ffbe0bfc3c"),
                        name: "SecondAccrual", userId: "unittest|784734738", startingHours: 27.5,
                        accrualRate: 7.66, maxHours: 255,
                        accrualFrequency: AccrualFrequency.SemiMonthly, dayOfPayA: 5, dayOfPayB: 22,
                        startingDate: new DateTime(2018, 12, 1), ending: Ending.CurrentYear, isHeart: false,
                        isArchived: false, minHours: 40, lastModified: new DateTime(2018, 11, 30, 22, 58, 11, 933), hourlyRate: 15,
                        actions: new List<AccrualActionRecord>
                        {
                            new AccrualActionRecord(
                                accrualActionId: "a7afbbb5-37a0-47c4-a12e-b6da40f8a7f6",
                                accrualAction: AccrualAction.Created,
                                actionDate: null,
                                amount: null, note: null, dateCreated: new DateTime(2018, 11, 30)),
                            new AccrualActionRecord(
                                accrualActionId: "58ab25e4-7262-4fef-9e79-e4f080e67714",
                                accrualAction: AccrualAction.Adjustment,
                                actionDate: new DateTime(2018, 12, 5),
                                amount: 8, note: "my test note", dateCreated: new DateTime(2018, 12, 1)),
                            new AccrualActionRecord(
                                accrualActionId: "b879ca33-77a6-460c-a346-b586ba33ac31",
                                accrualAction: AccrualAction.Adjustment,
                                actionDate: new DateTime(2018, 12, 12),
                                amount: 8, note: "note from december 12th",
                                dateCreated: new DateTime(2018, 12, 8, 4, 16, 37, 123)),
                        })
                },
            };
            var dashboardRepository = new Mock<IDashboardRepository>();
            dashboardRepository.Setup(_ => _.AddAccrual(It.IsAny<Accrual>())).Returns(
                (Accrual a) =>
                {
                    accruals.Add(a.AccrualId, a);
                    return Task.FromResult(true);
                });
            dashboardRepository.Setup(_ => _.GetAccrualForUserByAccrualIdAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(
                    (string userId, Guid accrualIdArg) =>
                    {
                        var result = accruals.TryGetValue(accrualIdArg, out var a);
                        if (result)
                        {
                            if (a.UserId == userId)
                            {
                                return Task.FromResult(a);
                            }
                        }

                        return null;
                    });
            dashboardRepository.Setup(_ =>
                _.AddActionForAccrualAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<AccrualActionRecord>())).Returns(
                (string userId, Guid accrualId, AccrualActionRecord action) =>
                {
                    var result = accruals.TryGetValue(accrualId, out var a);
                    if (result)
                    {
                        a.Actions.Add(action);
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                });

            dashboardRepository.Setup(_ =>
                _.DeleteActionAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
                (string userId, Guid accrualId, Guid accrualActionId) =>
                {
                    var result = accruals.TryGetValue(accrualId, out var a);
                    if (result)
                    {
                        int idx  = a.Actions.FindIndex(x => x.AccrualActionId == accrualActionId.ToString());
                        if (idx >= 0)
                        {
                            a.Actions.RemoveAt(idx);
                            return Task.FromResult(true);
                        }
                    }

                    return Task.FromResult(false);
                }
            );

            _defaults.Add(typeof(IDashboardRepository), dashboardRepository);
        }
    }

    [TestFixture]
    public class QueryQLMutations_Tests : QueryTestBase<TestMutationSchema>
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Services.Singleton(MockService.GetService<IAirportRepository>());
            Services.Singleton(MockService.GetService<IHttpContextAccessor>());
            Services.Singleton(MockService.GetService<IDotNetProvider>());
            Services.Singleton(MockService.GetService<IUserRepository>());
            Services.Singleton(MockService.GetService<IRoleRepository>());
            Services.Singleton(MockService.GetService<IDashboardRepository>());

            Services.Singleton(new TestMutationSchema(new FuncDependencyResolver(type => Services.Get(type))));
        }

        [Test]
        public void AddActionToMutation()
        {
            string query = @"
                mutation($accrualId: ID!, $action: AccrualActionRecordInputType!) {
                    addAccrualAction(accrualId: $accrualId, action: $action) {
                        accrualId name startingHours accrualRate maxHours accrualFrequency ending startingDate dayOfPayA dayOfPayB
                        actions {
                            accrualActionId actionDate accrualAction amount note dateCreated
                        }
                    }
                }
            ";

            string expected = @"{
                addAccrualAction: {
                    accrualId: '15548dda-9447-425d-ad7a-c19ffb000621',
                    name: 'AccrualForAddAction',
                    startingHours: 27.5,
                    accrualRate: 7.66,
                    maxHours: 255.0,
                    accrualFrequency: 'SEMIMONTHLY',
                    ending: 'CURRENTYEAR',
                    startingDate: '2018-12-01',
                    dayOfPayA: 5,
                    dayOfPayB: 22,
                    actions: [ {
                            accrualActionId: '5ecee8e4-36ca-4a4b-bc2e-c1aebc5c6b3f',
                            actionDate: null,
                            accrualAction: 'CREATED',
                            amount: null,
                            note: null,
                            dateCreated: '2018-11-30T00:00:00'
                        }, 
                        {
                            accrualActionId: '9d2383fd-4f83-4982-a889-9b397170168e',
                            actionDate: '2018-12-05',
                            accrualAction: 'ADJUSTMENT',
                            amount: 8.0,
                            note: 'some pto I used',
                            dateCreated: '2018-10-05T18:01:58.999'
                        }
                    ]
                }
            }";

            var data = new Dictionary<string, object>();
            data.Add("accrualId", "15548dda-9447-425d-ad7a-c19ffb000621");
            data.Add("action", new Dictionary<string, object>
            {
                {"actionDate", "2018-12-05"},
                {"amount", 8},
                {"accrualAction", "ADJUSTMENT"},
                {"note", "some pto I used"},
            });

            var inputs = new Inputs(data);

            AssertQuerySuccess(query, expected, inputs);
        }

        [Test]
        public void DeleteActionToMutation()
        {
            string query = @"
                mutation($accrualId: ID!, $accrualActionId: String!) {
                    deleteAccrualAction(accrualId: $accrualId, accrualActionId: $accrualActionId) {
                        accrualId name startingHours accrualRate maxHours accrualFrequency ending startingDate dayOfPayA dayOfPayB
                        actions {
                            accrualActionId actionDate accrualAction amount note dateCreated
                        }
                    }
                }
            ";

            string expected = @"{
                deleteAccrualAction: {
                    accrualId: '1851350b-0ce1-432b-a453-f6ffbe0bfc3c',
                    name: 'SecondAccrual',
                    startingHours: 27.5,
                    accrualRate: 7.66,
                    maxHours: 255.0,
                    accrualFrequency: 'SEMIMONTHLY',
                    ending: 'CURRENTYEAR',
                    startingDate: '2018-12-01',
                    dayOfPayA: 5,
                    dayOfPayB: 22,
                    actions: [ {
                            accrualActionId: 'a7afbbb5-37a0-47c4-a12e-b6da40f8a7f6',
                            actionDate: null,
                            accrualAction: 'CREATED',
                            amount: null,
                            note: null,
                            dateCreated: '2018-11-30T00:00:00'
                        }, 
                        {
                            accrualActionId: 'b879ca33-77a6-460c-a346-b586ba33ac31',
                            actionDate: '2018-12-12',
                            accrualAction: 'ADJUSTMENT',
                            amount: 8.0,
                            note: 'note from december 12th',
                            dateCreated: '2018-12-08T04:16:37.123'
                        }
                    ]
                }
            }";

            var data = new Dictionary<string, object>();
            data.Add("accrualId", "1851350b-0ce1-432b-a453-f6ffbe0bfc3c");
            data.Add("accrualActionId", "58ab25e4-7262-4fef-9e79-e4f080e67714");

            var inputs = new Inputs(data);

            AssertQuerySuccess(query, expected, inputs);
        }

        [Test]
        public void AddAccrualMutation()
        {
            string query = @"
                mutation($accrual: AccrualInputType!) {
                    addAccrual(accrual: $accrual) {
                        accrualId name startingHours accrualRate maxHours accrualFrequency ending startingDate dayOfPayA dayOfPayB
                        actions {
                            accrualActionId actionDate accrualAction amount note dateCreated
                        }
                        user {
                            userId
                        }
                    }
                }
            ";

            string expected = @"{
                addAccrual: {
                    accrualId: '9d2383fd-4f83-4982-a889-9b397170168e',
                    name: 'MutatedName',
                    startingHours: 27.5,
                    accrualRate: 7.66,
                    maxHours: 255.0,
                    accrualFrequency: 'SEMIMONTHLY',
                    ending: 'CURRENTYEAR',
                    startingDate: '2018-10-01',
                    dayOfPayA: 5,
                    dayOfPayB: 22,
                    actions: [ {
                            accrualActionId: '9d2383fd-4f83-4982-a889-9b397170168e',
                            actionDate: null,
                            accrualAction: 'CREATED',
                            amount: null,
                            note: null,
                            dateCreated: '2018-10-05T18:01:58.999'
                        }
                    ],
                    user: {
                        userId: 'unittest|784734738'
                    }
                }
            }";

            var data = new Dictionary<string, object>();
            data.Add("accrual", new Dictionary<string, object>
            {
                {"name", "MutatedName"},
                {"startingHours", "27.5"},
                {"accrualRate", "7.66"},
                {"maxHours", "255"},
                {"accrualFrequency", "SEMIMONTHLY"},
                {"startingDate", "2018-10-01"},
                {"ending", "CURRENTYEAR"},
                {"dayOfPayA", "5"},
                {"dayOfPayB", "22"},
            });

            var inputs = new Inputs(data);

            AssertQuerySuccess(query, expected, inputs);
        }

        [Test]
        public void AddAirportMutation()
        {
            string query = @"
                mutation($airport: AirportInputType!) {
                    addAirport(airport: $airport) {
                        code name
                    }
                }
            ";

            string expected = @"{
                addAirport: {
                    code: 'DEN', name: 'Denver International Airport'
                }
            }";

            var data = new Dictionary<string, object>();
            data.Add("airport", new Dictionary<string, object>
            {
                {"code", "DEN"},
                {"name", "Denver International Airport"}
            });

            var inputs = new Inputs(data);

            AssertQuerySuccess(query, expected, inputs);
        }
    }
}