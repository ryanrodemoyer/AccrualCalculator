using System;

namespace AppName.Web.Models
{
    public class AccrualActionRecordInput
    {
        public AccrualAction AccrualAction { get; set; }
        
        public DateTime ActionDate { get; set; }
        
        public double Amount { get; set; }
        
        public string Note { get; set; }

        public AccrualActionRecordInput()
        {
            
        }

        public AccrualActionRecordInput(AccrualAction accrualAction, DateTime actionDate, double amount, string note)
        {
            AccrualAction = accrualAction;
            ActionDate = actionDate;
            Amount = amount;
            Note = note;
        }
    }
}