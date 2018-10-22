using System;
using System.Collections.Generic;

namespace AppName.Web.Models
{
    public class IndexDashboardViewModel
    {
        public IEnumerable<Accrual> UserAccruals { get; set; }
    }
    
    public class ViewAccrualViewModel
    {
        public List<Guid> AllAccruals { get; set; }
        public Accrual Accrual { get; set; }

        public List<AccrualRow> AccrualRows { get; set; }
    }
    
    public class AccrualRow
    {
        public int RowId { get; set; }
        public DateTime? AccrualDate { get; set; }
        public double HoursUsed { get; set; }
        public double CurrentAccrual { get; set; }

        public IEnumerable<AccrualActionRecord> Actions { get; set; }

        public AccrualRow(int rowId, DateTime? accrualDate, double hoursUsed, double currentAccrual, IEnumerable<AccrualActionRecord> actions)
        {
            RowId = rowId;
            AccrualDate = accrualDate;
            HoursUsed = hoursUsed;
            CurrentAccrual = currentAccrual;
            Actions = actions ?? new List<AccrualActionRecord>();
        }
    }
}