using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercises.Data;
using StudentExercises.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudentExercises.Controllers
{
    [Route("api/[controller]")]
    public class InstructorsController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult<List<Instructor>> Get(string q, string _include, string active)
        {
            var instructors = new Repository(_config).GetInstructorsWithCohort(q, _include, active);
            return Ok(instructors);
        }

        [HttpGet("{id}", Name = "GetInstructor")]
        public ActionResult<Instructor> Get([FromRoute] int id)
        {
            var instructor = new Repository(_config).GetOneInstructor(id);

            if (instructor.Id > 0)
            {
                return Ok(instructor);
            }
            else
            {
                return NotFound($"Instructor with the id {id} was not found :(");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Instructor instructor)
        {
            new Repository(_config).InsertInstructorWithAssign(instructor.FirstName, instructor.LastName, instructor.SlackHandle, instructor.cohort.CohortNum);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult<Instructor> Put([FromRoute] int id, [FromBody] Instructor instructor)
        {
            var updatedInstructor = new Repository(_config).UpdateInstructor(id, instructor);

            return Ok(updatedInstructor);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            new Repository(_config).DeleteInstructor(id);

            return Ok();
        }
    }
}
