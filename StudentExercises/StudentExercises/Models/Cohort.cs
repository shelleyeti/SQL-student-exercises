using System;
namespace StudentExercises.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        public bool IsDayTime { get; set; }
        public int CohortNum { get; set; }
        public string CohortName { get; set; }
    }
}
