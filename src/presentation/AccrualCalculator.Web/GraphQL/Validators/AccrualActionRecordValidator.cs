using System.Collections.Generic;
using AppName.Web.Models;

namespace AppName.Web.GraphQL.Validators
{
    public static class AccrualActionRecordValidator
    {
        public static bool TryValidate(AccrualActionRecordInput action, out IList<ValidationError> errors)
        {
            errors = new List<ValidationError>();

            if (action == null)
            {
                errors.Add(new ValidationError("NullInput", "Input value is null."));
            }
            else
            {
                if (action.AccrualAction != AccrualAction.Created)
                {
                    if (action.Amount == 0d)
                    {
                        errors.Add(new ValidationError("Required", "Amount is required when Action is Adjustment."));
                    }
                    else if (action.Amount < -40 || action.Amount > 40)
                    {
                        errors.Add(new ValidationError("Range", "Amount must be between -40 and 40, inclusive, when Action is Adjustment."));                        
                    }
                }

                if (!string.IsNullOrWhiteSpace(action.Note))
                {
                    if (action.Note.Length > 100)
                    {
                        errors.Add(new ValidationError("MaxLength", "Maximum number of characters for Note is 100."));
                    }
                }
            }

            return errors.Count == 0;
        }
    }
}