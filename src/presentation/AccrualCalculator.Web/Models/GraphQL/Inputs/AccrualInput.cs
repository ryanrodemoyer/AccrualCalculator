using System;
using System.ComponentModel.DataAnnotations;

namespace AppName.Web.Models
{
    public class AccrualInput
    {
        // begin required
        public string Name { get; set; }        
        public double StartingHours { get; set; }
        public double AccrualRate { get; set; }
        public DateTime StartingDate { get; set; }
        public AccrualFrequency AccrualFrequency { get; set; }
        public Ending Ending { get; set; }
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
    }
}