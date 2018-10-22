using AppName.Web.Models;
using AppName.Web.Repositories;
using AppName.Web.Services;
using GraphQL.Types;

namespace AppName.Web.GraphQL
{
    public class AccrualGraphType : ObjectGraphType<Accrual>
    {
        private readonly IUserRepository _userRepository;
        private readonly AccrualService _accrualService;

        public AccrualGraphType(
            IUserRepository userRepository,
            AccrualService accrualService)
        {
            _userRepository = userRepository;
            _accrualService = accrualService;

            Field(x => x.AccrualId, type: typeof(IdGraphType)).Description("");
            Field(x => x.Name).Description("The name of the accrual chart.");
            Field(x => x.StartingHours).Description("Amount of hours to begin the accrual.");
            Field(x => x.AccrualRate).Description("Amount of hours earned per accrual period.");
            Field(x => x.StartingDate, type: typeof(DateGraphType)).Description("");
            Field(x => x.AccrualFrequency, type: typeof(AccrualFrequencyEnum)).Description("");
            Field(x => x.Ending, type: typeof(EndingEnum)).Description("");
            Field(x => x.LastModified, type: typeof(DateTimeGraphType)).Description("Timestamp for when last modified.");
            Field(x => x.IsHeart).Description("Is the accrual hearted?");
            Field(x => x.IsArchived).Description("Is the accrual archived?");

            Field(x => x.HourlyRate, nullable: true).Description("Amount of money earned per accrued hour.");
            Field(x => x.DayOfPayA, nullable: true).Description("");
            Field(x => x.DayOfPayB, nullable: true).Description("");
            Field(x => x.MinHours, nullable: true).Description("Minimum amount of hours the user wants to always have available.");
            Field(x => x.MaxHours, nullable: true).Description("Maximum possible amount of accrued hours.");
            
            Field<ListGraphType<AccrualActionRecordGraphType>>("actions", "");

            Field<UserGraphType>("user", "",
                resolve: context =>
                {
                    AppUser user = _userRepository.GetUserByUserIdAsync(context.Source.UserId).Result;
                    return user;
                });

            Field<ListGraphType<AccrualRowGraphType>>("rows", "",
                resolve: context =>
                {
                    var rows = _accrualService.Calculate(context.Source);
                    return rows;
                });
        }
    }
}