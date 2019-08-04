using System;
using System.Collections.Generic;

namespace StudentExercises.Models
{
    public class Student
    {
        private Cohort _cohort = null;
        private List<Exercise> _exerciseList = null;

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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