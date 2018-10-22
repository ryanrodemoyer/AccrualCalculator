using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using AppName.Web.Extensions;
using AppName.Web.GraphQL.Validators;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Repositories;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace AppName.Web.GraphQL
{
    public class AppMutation : ObjectGraphType<object>
    {
        private readonly IAirportRepository _airportRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDotNetProvider _dotNetProvider;

        public AppMutation(
            IAirportRepository airportRepository,
            IDashboardRepository dashboardRepository,
            IHttpContextAccessor httpContextAccessor,
            IDotNetProvider dotNetProvider)
        {
            _airportRepository = airportRepository;
            _dashboardRepository = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _dotNetProvider = dotNetProvider;

            Field<AirportGraphType>("addAirport", "",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AirportInputType>>
                        {Name = "airport", Description = "The airport to add."}
                ),
                resolve: context =>
                {
                    AirportInput airport = context.GetArgument<AirportInput>("airport");
                    Airport a = _airportRepository.GetAirport(airport.Code);
                    if (a == null)
                    {
                        _airportRepository.AddAirport(airport);

                        return _airportRepository.GetAirport(airport.Code);
                    }

                    return a;
                });

            Field<AccrualGraphType>("addAccrualAction", "",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                        {Name = "accrualId", Description = "The accrual to modify."},
                    new QueryArgument<NonNullGraphType<AccrualActionRecordInputType>>
                        {Name = "action", Description = "The action to add."}
                ),
                resolve: context =>
                {
                    AccrualActionRecordInput input = context.GetArgument<AccrualActionRecordInput>("action");

                    if (AccrualActionRecordValidator.TryValidate(input, out var errors))
                    {
                        string accrualIdArgument = context.GetArgument<string>("accrualId");
                        Guid.TryParse(accrualIdArgument, out var id);
                        string userId = _httpContextAccessor.HttpContext.User.UserId();

                        var action = new AccrualActionRecord(_dotNetProvider.NewGuid.ToString(), input.AccrualAction,
                            input.ActionDate, input.Amount,
                            input.Note, _dotNetProvider.DateTimeNow);

                        bool added = _dashboardRepository.AddActionForAccrualAsync(userId, id, action).Result;
                        if (added)
                        {
                            return _dashboardRepository.GetAccrualForUserByAccrualIdAsync(userId, id);
                        }

                        context.Errors.Add(new ExecutionError("Error adding new accrual."));
                        return null;
                    }

                    context.Errors.Add(new ExecutionError(string.Join(Environment.NewLine, errors)));
                    return null;
                });

            Field<AccrualGraphType>("deleteAccrualAction", "",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                        {Name = "accrualId", Description = "The accrual to modify."},
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                        {Name = "accrualActionId", Description = "The action to remove."}
                ),
                resolve: context =>
                {
                    string accrualIdArgument = context.GetArgument<string>("accrualId");
                    string accrualActionIdArgument = context.GetArgument<string>("accrualActionId");

                    Guid.TryParse(accrualIdArgument, out var accrualId);
                    Guid.TryParse(accrualActionIdArgument, out var accrualActionId);
                    string userId = _httpContextAccessor.HttpContext.User.UserId();

                    bool removed = _dashboardRepository.DeleteActionAsync(userId, accrualId, accrualActionId).Result;
                    if (removed)
                    {
                        return _dashboardRepository.GetAccrualForUserByAccrualIdAsync(userId, accrualId);
                    }

                    context.Errors.Add(new ExecutionError("Error removing accrual action."));
                    return null;
                });

            Field<AccrualGraphType>("modifyAccrual", "",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                        {Name = "accrualId", Description = "The accrual to add."},
                    new QueryArgument<NonNullGraphType<AccrualInputType>>
                        {Name = "accrual", Description = "The accrual to update."}
                ),
                resolve: context =>
                {
                    string accrualIdArgument = context.GetArgument<string>("accrualId");
                    var accrualInput = context.GetArgument<AccrualInput>("accrual");
                    string userId = _httpContextAccessor.HttpContext.User.UserId();

                    Guid.TryParse(accrualIdArgument, out var accrualId);

                    bool result = AccrualValidator.TryValidate(accrualInput, out var errors);
                    if (result)
                    {
                        var accrual = _dashboardRepository.GetAccrualForUserByAccrualIdAsync(userId, accrualId).Result;

                        accrual.Name = accrualInput.Name;
                        accrual.StartingHours = accrualInput.StartingHours;
                        accrual.AccrualRate = accrualInput.AccrualRate;
                        accrual.StartingDate = accrualInput.StartingDate;
                        accrual.Ending = accrualInput.Ending;
                        accrual.AccrualFrequency = accrualInput.AccrualFrequency;
                        accrual.LastModified = _dotNetProvider.DateTimeNow;
                        accrual.HourlyRate = accrualInput.HourlyRate;
                        accrual.MinHours = accrualInput.MinHours;
                        accrual.MaxHours = accrualInput.MaxHours;
                        accrual.IsHeart = accrualInput.IsHeart;
                        accrual.IsArchived = accrualInput.IsArchived;
                        accrual.DayOfPayA = accrualInput.AccrualFrequency == AccrualFrequency.SemiMonthly
                            ? accrualInput.DayOfPayA
                            : null;
                        accrual.DayOfPayB = accrualInput.AccrualFrequency == AccrualFrequency.SemiMonthly
                            ? accrualInput.DayOfPayB
                            : null;
                        
                        bool modified = _dashboardRepository.ModifyAccrualAsync(accrual).Result;
                        if (modified)
                        {
                            return _dashboardRepository.GetAccrualForUserByAccrualIdAsync(userId, accrualId);
                        }

                        context.Errors.Add(new ExecutionError("Failed to update accrual."));
                        return null;
                    }
                    
                    context.Errors.Add(new ExecutionError(string.Join(Environment.NewLine, errors)));
                    return null;
                });

            Field<AccrualGraphType>("addAccrual", "",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AccrualInputType>>
                        {Name = "accrual", Description = "The accrual to add."}
                ),
                resolve: context =>
                {
                    AccrualInput input = context.GetArgument<AccrualInput>("accrual");

                    bool result = AccrualValidator.TryValidate(input, out var errors);
                    if (result)
                    {
                        string userId = _httpContextAccessor.HttpContext.User.UserId();
                        Guid accrualId = _dotNetProvider.NewGuid;
                        DateTime now = _dotNetProvider.DateTimeNow;

                        var a = new Accrual
                        {
                            UserId = userId,
                            AccrualId = accrualId,
                            Name = input.Name,
                            StartingHours = input.StartingHours,
                            AccrualRate = input.AccrualRate,
                            StartingDate = input.StartingDate,
                            Ending = input.Ending,
                            AccrualFrequency = input.AccrualFrequency,
                            LastModified = _dotNetProvider.DateTimeNow,
                            HourlyRate = input.HourlyRate,
                            IsHeart = input.IsHeart,
                            IsArchived = input.IsArchived,
                            MinHours = input.MinHours,
                            MaxHours = input.MaxHours,
                            DayOfPayA = input.AccrualFrequency == AccrualFrequency.SemiMonthly ? input.DayOfPayA : null,
                            DayOfPayB = input.AccrualFrequency == AccrualFrequency.SemiMonthly ? input.DayOfPayB : null,
                            Actions = new List<AccrualActionRecord>
                            {
                                new AccrualActionRecord(_dotNetProvider.NewGuid.ToString(), AccrualAction.Created, null,
                                    null, null, now)
                            }
                        };

                        bool added = _dashboardRepository.AddAccrual(a).Result;
                        if (added)
                        {
                            return _dashboardRepository.GetAccrualForUserByAccrualIdAsync(userId, accrualId);
                        }

                        context.Errors.Add(new ExecutionError("Error adding new accrual."));
                        return null;
                    }

                    context.Errors.Add(new ExecutionError(string.Join(Environment.NewLine, errors)));
                    return null;
                });
        }
    }

//    public class UpdateArgs<TBase, TField>
//    {
//        public Expression<Func<TBase,TField>> Field { get; set; }
//        public TField Value { get; set; }
//
//        public UpdateArgs(Expression<Func<TBase, TField>> field, TField value)
//        {
//            Field = field;
//            Value = value;
//        }
//    }
}