using System;
using System.Linq;
using AppName.Web.Extensions;
using AppName.Web.Models;
using AppName.Web.Repositories;
using GraphQL.Types;
using GraphQLParser;
using Microsoft.AspNetCore.Http;

namespace AppName.Web.GraphQL
{
    public class UserGraphType : ObjectGraphType<AppUser>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserGraphType(
            IRoleRepository roleRepository,
            IDashboardRepository dashboardRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _roleRepository = roleRepository;
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;

            Field(x => x.UserId, type: typeof(IdGraphType)).Description("Unique identifier from the authentication provider.");
            Field(x => x.DateCreated, type: typeof(DateTimeGraphType)).Description("The date the record was created in the system.");

//            Field<ListGraphType<RoleGraphType>>("roles",
//                description: "",
//                resolve: context =>
//                {
//                    var results = context.Source.RoleIds.Select(x => _roleRepository.GetRoleByIdAsync(x).Result)
//                        .ToList();
//                    return results;
//                });

            Field<ListGraphType<AccrualGraphType>>("accruals",
                description: "",
                resolve: context =>
                {
//                    int userId = _httpContextAccessor.HttpContext.User.UserId();
                    var results = _dashboardRepository.GetAllAccrualsForUser(context.Source.UserId);
                    return results;
                });
        }
    }
}