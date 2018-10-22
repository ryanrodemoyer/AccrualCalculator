using System;
using System.Collections.Generic;
using AppName.Web.Models;

namespace AppName.Web.GraphQL.Validators
{
    public static class AccrualValidator
    {
        public static bool TryValidate(AccrualInput accrual, out IList<ValidationError> errors)
        {
            errors = new List<ValidationError>();

            if (accrual == null)
            {
                errors.Add(new ValidationError("NullInput", "Input value is null."));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(accrual.Name))
                {
                    errors.Add(new ValidationError("Required", "Name is required."));
                }

                if (accrual.Name?.Length > 100)
                {
                    errors.Add(new ValidationError("MaxLength", "Name must be 100 characters or less."));
                }
                
                if (accrual.StartingHours <= -101)
                {
                    errors.Add(new ValidationError("RangeError", "AccrualRate must be greater than or equal to -101."));
                }
                
                if (accrual.AccrualRate <= 0)
                {
                    errors.Add(new ValidationError("RangeError", "AccrualRate must be greater than zero."));
                }
                
                if (accrual.StartingDate.Year != DateTime.Now.Year)
                {
                    errors.Add(new ValidationError("YearError", "StartingDate must be within the current calendar year."));
                }
                
                if (accrual.AccrualFrequency == AccrualFrequency.SemiMonthly)
                {
                    int dayA = accrual.DayOfPayA.GetValueOrDefault();
                    int dayB = accrual.DayOfPayB.GetValueOrDefault();

                    if (dayA <= 0 || dayA >= 32)
                    {
                        errors.Add(new ValidationError("RangeError", "DayOfPayA must be between 1 and 31 inclusive."));
                    }

                    if (dayB <= 0 || dayB >= 32)
                    {
                        errors.Add(new ValidationError("RangeError", "DayOfPayB must be between 1 and 31 inclusive."));
                    }
                }

                if (accrual.HourlyRate <= 0)
                {
                    errors.Add(new ValidationError("RangeError", "HourlyRate must be null or greater than zero."));
                }

                if (accrual.MinHours <= 0)
                {
                    errors.Add(new ValidationError("RangeError", "MinHours must be greater than zero."));
                }
                
                if (accrual.MaxHours <= 0)
                {
                    errors.Add(new ValidationError("RangeError", "MaxHours must be greater than zero."));
                }
            }

            return errors.Count == 0;
        }
    }
}