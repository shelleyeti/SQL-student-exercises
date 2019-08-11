using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercises.Models;
using StudentExercises.Data;


namespace StudentExercises.Controllers
{
    [Route("api/[controller]")]
    public class CohortsController : Controller
    {
        private readonly IConfiguration _config;

        public CohortsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult<List<Cohort>> Get(string q, string _include, string active)
        {
            var cohorts = new Repository(_config).GetAllCohorts(q, _include, active);
            return Ok(cohorts);
        }

        [HttpGet("{id}", Name = "GetCohort")]
        public ActionResult<Student> Get([FromRoute] int id)
        {
            var cohort = new Repository(_config).GetOneCohort(id);

            if (cohort.Id > 0)
            {
                return Ok(cohort);
            }
            else
            {
                return NotFound($"Cohort with the id {id} was not found :(");
            }
        }

        [HttpPost]
        public ActionResult<Cohort> Post([FromBody] Cohort cohort)
        {
            var newCohort = new Repository(_config).AddCohort(cohort);
            return Ok(newCohort);
        }

        [HttpPut("{id}")]
        public ActionResult<Cohort> Put([FromRoute] int id, [FromBody] Cohort cohort)
        {
            var updatedCohort = new Repository(_config).UpdateCohort(id, cohort);

            return Ok(updatedCohort);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            new Repository(_config).DeleteCohort(id);

            return Ok();
        }
    }
}
