using System.ComponentModel.DataAnnotations;

namespace StudentExercises.Models
{
    public class Instructor
    {
        private Cohort _cohort = null;
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string SlackHandle { get; set; }

        public Instructor()
        {
            _cohort = new Cohort();
        }
        public Cohort cohort
        {
            get { return _cohort; }
            set { _cohort = value; }
        }
    }
}
