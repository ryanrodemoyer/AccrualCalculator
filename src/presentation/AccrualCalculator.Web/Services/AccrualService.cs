using System;
using System.Collections.Generic;
using System.Linq;
using AppName.Web.Extensions;
using AppName.Web.Models;
using AppName.Web.Providers;

namespace AppName.Web.Services
{
    public class AccrualService
    {
        private readonly IDotNetProvider _dotNetProvider;

        public AccrualService(
            IDotNetProvider dotNetProvider)
        {
            _dotNetProvider = dotNetProvider;
        }

        public List<AccrualRow> Calculate(Accrual config)
        {
            List<AccrualRow> rows = new List<AccrualRow>();

            foreach (var action in config.Actions)
            {
                switch (action.AccrualAction)
                {
                    case AccrualAction.Created:
                        rows = CreateAccrualTable(config);
                        break;
                }
            }

            return rows;
        }

        private List<AccrualRow> CreateAccrualTable(Accrual config)
        {
            DateTime previous = config.StartingDate;
            DateTime current = config.StartingDate;
            DateTime end = config.GetEndDate(_dotNetProvider);

            var rows = new List<AccrualRow>
            {
                new AccrualRow(0, null, 0, config.StartingHours, null)
            };

            double accrual = config.StartingHours;

            int counter = 1;
            while (current <= end)
            {
                DateTime c = current, p = previous;
                var actions = config.Actions
                    .Where(a => a.AccrualAction != AccrualAction.Created)
                    .Where(a => a.ActionDate >= p && a.ActionDate < c);
                double sum = actions.Sum(x => x.Amount).GetValueOrDefault();

                accrual += config.AccrualRate + sum;

                rows.Add(new AccrualRow(counter++, current, sum, accrual, actions));

                previous = current;

                if (config.AccrualFrequency == AccrualFrequency.Biweekly)
                {
                    current = current.AddDays(14);
                }
                else
                {
                    int day = current.Day;
                    if (day >= config.DayOfPayB)
                    {
                        current = current.AddMonths(1);
                        current = new DateTime(current.Year, current.Month, config.DayOfPayA.Value);
                    }
                    else
                    {
                        current = new DateTime(current.Year, current.Month, config.DayOfPayB.Value);
                    }
                }
            }

            return rows;
        }
    }
}