using System;
namespace StudentExercises.Models
{
    public class Instructor
    {
        private Cohort _cohort = null;
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SlackHandle { get; set; }

        public Instructor()
        {
            _cohort = new Cohort();
        }
        public Cohort cohort
        {
            get { return _cohort; }
            set { _cohort = value;  }
        }
    }
}
