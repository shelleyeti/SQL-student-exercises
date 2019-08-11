using System;
using System.ComponentModel.DataAnnotations;

namespace StudentExercises.Models
{
    public class Cohort
    {
        public int Id { get; set; }

        [Required]
        public bool IsDayTime { get; set; }

        [Required]
        public int CohortNum { get; set; }

        [StringLength(11, MinimumLength = 5)]
        public string CohortName { get; set; }
    }
}
