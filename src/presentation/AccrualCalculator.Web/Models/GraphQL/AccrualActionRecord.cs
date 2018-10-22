using System;
using MongoDB.Bson;

namespace AppName.Web.Models
{
    public class AccrualActionRecord
    {
        public string AccrualActionId { get; set; }
        
        public AccrualAction AccrualAction { get; set; }
        
        public DateTime? ActionDate { get; set; }
        
        public double? Amount { get; set; }
        
        public string Note { get; set; }
        
        public DateTime DateCreated { get; set; }

        public AccrualActionRecord(string accrualActionId, AccrualAction accrualAction, DateTime? actionDate, double? amount, string note, DateTime dateCreated)
        {
            AccrualActionId = accrualActionId;
            AccrualAction = accrualAction;
            ActionDate = actionDate;
            Amount = amount;
            Note = note;
            DateCreated = dateCreated;
        }
    }
}