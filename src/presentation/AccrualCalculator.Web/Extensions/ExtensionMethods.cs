using System;
using System.Linq;
using System.Security.Claims;
using AppName.Web.Models;
using AppName.Web.Providers;

namespace AppName.Web.Extensions
{
    public static class ExtensionMethods
    {
        public static string UserId(this ClaimsPrincipal user)
        {
            Claim claim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                throw new InvalidOperationException("Missing NameIdentifier claim.");
            }

            return claim.Value;

//            bool result = int.TryParse(sidClaim.Value, out int id);
//            if (result)
//            {
//                return id;
//            }
//            
//            throw new InvalidOperationException("NameIdentifier claim not convertable to int");
        }
        public static bool HasApiAccess(this ClaimsPrincipal user)
        {
            Claim claim = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role & x.Value == "Access.Api");
            if (claim == null)
            {
                return false;
            }

            return true;
        }
    }
    
    public static class AccrualExtensions
    {
        public static DateTime GetEndDate(this Accrual accrual, IDotNetProvider dotNetProvider)
        {
            int currentYear = dotNetProvider.DateTimeNow.Year;
            DateTime end = new DateTime(currentYear, 12, 31);
            
            switch (accrual.Ending)
            {
                case Ending.PlusOne:
                    end = end.AddYears(1);
                    break;
                case Ending.PlusTwo:
                    end = end.AddYears(2);
                    break;
                case Ending.PlusThree:
                    end = end.AddYears(3);
                    break;
            }

            return end;
        }
    }
}