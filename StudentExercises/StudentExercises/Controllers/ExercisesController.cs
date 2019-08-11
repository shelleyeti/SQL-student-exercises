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
    public class ExercisesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ExercisesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult<List<Exercise>> Get(string q, string _include, string active)
        {
            var exercises = new Repository(_config).GetAllExercises(q, _include, active);
            return Ok(exercises);
        }

        [HttpGet("{language}", Name = "GetByLanguage")]
        public ActionResult<List<Exercise>> Get([FromRoute] string language)
        {
            var exercises = new Repository(_config).GetByLanguage(language);
            return Ok(exercises);
        }

        //[HttpGet("{id}", Name = "GetOneExercise")]
        //public ActionResult <Exercise> Get([FromRoute] int id)
        //{
        //    var exercises = new Repository(_config).GetOneExercise(id);
        //    return Ok(exercises);
        //}

        [HttpPost]
        public ActionResult Post([FromBody] Exercise exercise)
        {
            new Repository(_config).AddExercise(exercise.ExName, exercise.ExLanguage);
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult<Exercise> Put([FromRoute] int id, [FromBody] Exercise exercise)
        {
            var updatedExercise = new Repository(_config).UpdateExercise(id, exercise);

            return Ok(updatedExercise);
        }

        [HttpPost("{exerciseId}")]
        public ActionResult Post([FromRoute] int exerciseId, [FromBody] Cohort cohort)
        {
            new Repository(_config).AssignCohortExercises(exerciseId, cohort.Id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            new Repository(_config).DeleteExercise(id);

            return Ok();
        }
    }
}