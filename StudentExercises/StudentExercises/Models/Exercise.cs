using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentExercises.Models
{
    public class Exercise
    {
        [Required]
        [StringLength(25)]
        public string ExName { get; set; }

        [Required]
        [StringLength(25)]
        public string ExLanguage { get; set; }
        public int Id { get; set; }
        public List<Student> studentList { get; set; }
    }
}
