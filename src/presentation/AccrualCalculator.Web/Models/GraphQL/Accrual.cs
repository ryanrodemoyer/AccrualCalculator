using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace AppName.Web.Models
{
    public class Accrual
    {
        // begin not-exposed
        public ObjectId _id { get; set; }
        // end not-exposed
        
        // begin required
        public Guid AccrualId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }        
        public double StartingHours { get; set; }
        public double AccrualRate { get; set; }      
        public DateTime StartingDate { get; set; }
        public AccrualFrequency AccrualFrequency { get; set; }        
        public Ending Ending { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsHeart { get; set; }
        public bool IsArchived { get; set; }
        // end required
        
        // begin optional
        public decimal? HourlyRate { get; set; }
        public int? DayOfPayA { get; set; }
        public int? DayOfPayB { get; set; }
        public double? MinHours { get; set; }
        public double? MaxHours { get; set; }
        // end optional
        
        public List<AccrualActionRecord> Actions { get; set; }

        public Accrual()
        {
            
        }

        public Accrual(Guid accrualId, string userId, string name, double startingHours, double accrualRate, DateTime startingDate, AccrualFrequency accrualFrequency, Ending ending, DateTime lastModified, bool isHeart, bool isArchived, decimal? hourlyRate, int? dayOfPayA, int? dayOfPayB, double? minHours, double? maxHours, List<AccrualActionRecord> actions)
        {
            AccrualId = accrualId;
            UserId = userId;
            Name = name;
            StartingHours = startingHours;
            AccrualRate = accrualRate;
            StartingDate = startingDate;
            AccrualFrequency = accrualFrequency;
            Ending = ending;
            LastModified = lastModified;
            IsHeart = isHeart;
            IsArchived = isArchived;
            HourlyRate = hourlyRate;
            DayOfPayA = dayOfPayA;
            DayOfPayB = dayOfPayB;
            MinHours = minHours;
            MaxHours = maxHours;
            Actions = actions ?? new List<AccrualActionRecord>();
        }
    }
}