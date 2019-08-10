using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpPost]
        public ActionResult Post([FromBody] Instructor instructor)
        {
            new Repository(_config).InsertInstructorWithAssign(instructor.FirstName, instructor.LastName, instructor.SlackHandle, instructor.cohort.CohortNum);
            return Ok();
        }
    }
}
