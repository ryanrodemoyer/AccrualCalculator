using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppName.Web.Models;

namespace AppName.Web.Repositories
{
    public interface IDashboardRepository
    {
        Task<bool> AddActionForAccrualAsync(string userId, Guid accrualId, AccrualActionRecord action);

        Task<bool> DeleteActionAsync(string userId, Guid accrualId, Guid accrualActionId);
        
        Task<List<Accrual>> GetAllAccrualsForUser(string userId);
        
        Task<Accrual> GetAccrualForUserByAccrualIdAsync(string userId, Guid accrualId);

        Task<bool> AddAccrual(Accrual accrual);
        
        Task<bool> ModifyAccrualAsync(Accrual accrual);
    }
}