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

namespace AppTests.AppName.Web.GraphQL
{
    public static class TestExtensions
    {
        public static void UseGraphQLForQueries(this ISimpleContainer container)
        {
            var dateTimeProvider = new Mock<IDotNetProvider>();
            dateTimeProvider.Setup(x => x.DateTimeNow).Returns(new DateTime(2018, 10, 10, 8, 8, 8, 27));

            var roles = new List<AppRole> {new AppRole(1, "TestRole")};
            var roleRepository = new Mock<IRoleRepository>();
            roleRepository.Setup(_ => _.GetAllRolesAsync()).Returns(
                Task.FromResult(roles)
            );
            roleRepository.Setup(_ => _.GetRoleByIdAsync(It.IsAny<int>())).Returns(
                (int roleId) => { return Task.FromResult(roles.FirstOrDefault(x => x.RoleId == roleId)); }
            );

            var airportRepository = new Mock<IAirportRepository>();
            airportRepository.Setup(_ => _.GetAirports()).Returns(
                new List<Airport> {new Airport("XYZ", "XYZ Regional Airport")}
            );
            airportRepository.Setup(_ => _.GetAirport(It.IsAny<string>())).Returns(
                (string code) =>
                {
                    var airports = new Dictionary<string, Airport>
                    {
                        {"XYZ", new Airport("XYZ", "XYZ Regional Airport")}
                    };
                    return airports[code];
                }
            );

            var users = new List<AppUser> {new AppUser("unittest|784734738", new DateTime(2018, 10, 22, 18, 6, 6, 565))};
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(_ => _.GetAllUsersAsync()).Returns(Task.FromResult(users));
            userRepository.Setup(_ => _.GetUserByUserIdAsync(It.IsAny<string>())).Returns(
                (string userId) => { return Task.FromResult(users.FirstOrDefault(x => x.UserId == userId)); }
            );

            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {new Claim(ClaimTypes.NameIdentifier, "unittest|784734738")}));
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            Guid accrualId = Guid.Parse("505F5343-AB40-4CE8-B54E-6B60C38F4588");
            var dashboardRepository = new Mock<IDashboardRepository>();
            dashboardRepository.Setup(_ => _.GetAccrualForUserByAccrualIdAsync("unittest|784734738", accrualId)).Returns(
                Task.FromResult(
                    new Accrual(accrualId: accrualId, name: "MockName", userId: "unittest|784734738", startingHours: 11, accrualRate: 7, maxHours: 255, accrualFrequency: AccrualFrequency.SemiMonthly, dayOfPayA: 7, dayOfPayB: 21, startingDate: new DateTime(2018, 10, 6),
                        ending: Ending.PlusThree, isHeart: false, isArchived: true, minHours: 12.75, lastModified: new DateTime(2018, 10, 10, 12, 37, 18, 555), hourlyRate: 39m,
                        actions: new List<AccrualActionRecord> {new AccrualActionRecord("cc1438bb-47f9-4a26-99fe-041d25ee79ca", AccrualAction.Created, null, null, null, new DateTime(2018, 10, 10, 12, 37, 18, 555))}))
            );

            container.Singleton(dashboardRepository.Object);
            container.Singleton(dateTimeProvider.Object);
            container.Singleton(roleRepository.Object);
            container.Singleton(airportRepository.Object);
            container.Singleton(userRepository.Object);
            container.Singleton(httpContextAccessor.Object);
            container.Register<AppQuery>();

            container.Singleton(new TestQuerySchema(new FuncDependencyResolver(type => container.Get(type))));
        }
    }
}