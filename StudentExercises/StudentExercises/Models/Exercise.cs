using System;
using System.Collections.Generic;

namespace StudentExercises.Models
{
    public class Exercise
    {
        public string ExName { get; set; }
        public string ExLanguage { get; set; }
        public int Id { get; internal set; }
    }
}
