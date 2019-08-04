﻿using System;
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
        public ActionResult<List<Exercise>> Get()
        {
            var exercises = new Repository(_config).GetAllExercises();
            return Ok(exercises);
        }

        [HttpGet("{language}", Name = "GetByLanguage")]
        public ActionResult<List<Exercise>> Get([FromRoute] string language)
        {
            var exercises = new Repository(_config).GetByLanguage(language);
            return Ok(exercises);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Exercise exercise)
        {
            new Repository(_config).AddExercise(exercise.ExName, exercise.ExLanguage);
            return Ok();
        }

    }
}