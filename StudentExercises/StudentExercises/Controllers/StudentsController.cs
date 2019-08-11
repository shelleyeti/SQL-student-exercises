using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using StudentExercises.Models;
using StudentExercises.Data;

namespace StudentExercises.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public ActionResult<List<Student>> Get([FromQuery] string q, string _include, string active)
        {
            var students = new Repository(_config).GetAllStudents(q, _include, active);

            return Ok(students);     
        }

        [HttpGet("{id}", Name = "GetStudent")]
        public ActionResult<Student> Get([FromRoute] int id)
        {
            var student = new Repository(_config).GetOneStudent(id);

            if (student.Id > 0 )
            {
            return Ok(student);
            }
            else
            {
                return NotFound($"Student with the id {id} was not found :(");
            }
        }

        [HttpPost]
        public ActionResult<Student> Post([FromBody] Student student)
        {
            var newStudent = new Repository(_config).AddStudent(student);
            return Ok(newStudent);
        }

        [HttpPut("{id}")]
        public ActionResult<Student> Put([FromRoute] int id, [FromBody] Student student)
        {
            var updatedStudent = new Repository(_config).UpdateStudent(id, student);

            return Ok(updatedStudent);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            new Repository(_config).DeleteStudent(id);

            return Ok();
        }

        [HttpPost("{exerciseName}")]
        public ActionResult Post([FromRoute] string exerciseName, [FromBody] Student student)
        {
            new Repository(_config).AddExerciseToStudent(student.FirstName, student.LastName, student.SlackHandle, exerciseName);

            return Ok();
        }


    }
}