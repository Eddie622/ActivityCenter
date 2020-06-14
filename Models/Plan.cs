using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PlannedActivityId { get; set; }
        public User User { get; set; }
        public PlannedActivity PlannedActivity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}