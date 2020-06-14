using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models
{
    public class PlannedActivity
    {
        [Key]
        public int PlannedActivityId { get; set; }

        [Required]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string Time { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [FutureDateValidation]
        public DateTime Date { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public string DurationLengthType { get; set; }

        [Required]
        public string Description { get; set; }

        public List<Plan> Participants { get; set; }

        [Required]
        public int UserId {get;set;}
        public User Creator {get;set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}