using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentExercises.Models
{
    public class Student
    {
        private Cohort _cohort = null;
        private List<Exercise> _exerciseList = null;

        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 3)]
        public string SlackHandle { get; set; }

        public Student()
        {
            _cohort = new Cohort();
            _exerciseList = new List<Exercise>();
        }
        public Cohort cohort
        {
            get { return _cohort; }
            set { _cohort = value; }
        }

        public List<Exercise> exerciseList
        {
            get { return _exerciseList; }
            set { _exerciseList = value; }
        }
    }
}