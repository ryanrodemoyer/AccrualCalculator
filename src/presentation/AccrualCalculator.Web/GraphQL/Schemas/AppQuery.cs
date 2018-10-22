using System;
using AppName.Web.Extensions;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Repositories;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;

namespace AppName.Web.GraphQL
{
    public class AppQuery : ObjectGraphType<object>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IDotNetProvider _dotNetProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppQuery(
            IRoleRepository roleRepository,
            IAirportRepository airportRepository,
            IUserRepository userRepository,
            IDashboardRepository dashboardRepository,
            IDotNetProvider dotNetProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _roleRepository = roleRepository;
            _airportRepository = airportRepository;
            _userRepository = userRepository;
            _dashboardRepository = dashboardRepository;
            _dotNetProvider = dotNetProvider;
            _httpContextAccessor = httpContextAccessor;

            Field<HealthCheckGraphType>("healthcheck",
                description: "Information about the status of the GraphQL API.",
                resolve: context => { return new Healthcheck("1.0.0", _dotNetProvider.DateTimeNow); }
            );

            Field<ListGraphType<RoleGraphType>>("roles",
                description: "Configuration: Roles available in the application.",
                resolve: context => { return _roleRepository.GetAllRolesAsync().Result; }
            );

            Field<ListGraphType<AirportGraphType>>("airports",
                description: "A list of airport codes and names.",
                resolve: context => { return _airportRepository.GetAirports(); });

            Field<AirportGraphType>("airport",
                description: "Information about a specific airport.",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> {Name = "code", Description = "The airport code."}
                ),
                resolve: context =>
                {
                    string codeArgument = context.GetArgument<string>("code");
                    return _airportRepository.GetAirport(codeArgument);
                });

            Field<ListGraphType<UserGraphType>>("users",
                description: "The users in the application.",
                resolve: context => { return _userRepository.GetAllUsersAsync().Result; });

//            Field<UserGraphType>("user",
//                description: "Information about a specific user.",
//                arguments: new QueryArguments(
//                    new QueryArgument<NonNullGraphType<IdGraphType>> {Name = "userId", Description = "The User ID."}
//                ),
//                resolve: context =>
//                {
//                    string userIdArgument = context.GetArgument<string>("userId");
//                    return _userRepository.GetUserByUserIdAsync(userIdArgument);
//                });

            Field<AccrualGraphType>("accrual",
                description: "",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                        {Name = "accrualId", Description = "The Accrual ID."}
                ),
                resolve: context =>
                {
                    string userId = _httpContextAccessor.HttpContext.User.UserId();
                    string accrualIdArgument = context.GetArgument<string>("accrualId");
                    Guid.TryParse(accrualIdArgument, out var id);
                    return _dashboardRepository.GetAccrualForUserByAccrualIdAsync(userId, id);
                });
        }
    }
}