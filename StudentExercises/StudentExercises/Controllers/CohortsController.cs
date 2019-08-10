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
    }
}
