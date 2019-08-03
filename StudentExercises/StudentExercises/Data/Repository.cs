using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExercises.Models;

namespace StudentExercises.Data
{
    public class Repository
    {
        private readonly IConfiguration _config;

        public Repository(IConfiguration config)
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

        /************************************************************************************
        * Query the database for all the Exercises.
        ************************************************************************************/
        public List<Exercise> GetAllExercises()
        {
            var exercises = new List<Exercise>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, ExName, ExLanguage FROM Exercise";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");

                        int idValue = reader.GetInt32(idColumnPosition);

                        var exName = reader.GetString(reader.GetOrdinal("ExName"));
                        var exLanguage = reader.GetString(reader.GetOrdinal("ExLanguage"));

                        Exercise exercise = new Exercise
                        {
                            Id = idValue,
                            ExName = exName,
                            ExLanguage = exLanguage
                        };

                        exercises.Add(exercise);
                    }

                    reader.Close();

                    return exercises;
                }
            }
        }

        /************************************************************************************
        * Find all the exercises in the database where the language is JavaScript.
        ************************************************************************************/
        public List<Exercise> GetByLanguage(string language)
        {
            var exercises = new List<Exercise>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ExName, ExLanguage
                                        FROM Exercise
                                        WHERE ExLanguage LIKE '%' + @language + '%'";

                    cmd.Parameters.Add(new SqlParameter("@language", language));
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        var exName = reader.GetString(reader.GetOrdinal("ExName"));
                        var exLanguage = reader.GetString(reader.GetOrdinal("ExLanguage"));

                        Exercise exercise = new Exercise
                        {
                            Id = idValue,
                            ExName = exName,
                            ExLanguage = exLanguage
                        };

                        exercises.Add(exercise);
                    }

                    reader.Close();

                    return exercises;
                }
            }
        }

        /************************************************************************************
        * Insert a new exercise into the database.
        ************************************************************************************/
        public void AddExercise(string exName, string exLanguage)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise
                                        (ExName, ExLanguage)
                                        VALUES (@exName, @exLanguage)";

                    cmd.Parameters.Add(new SqlParameter("@exName", exName));
                    cmd.Parameters.Add(new SqlParameter("@exLanguage", exLanguage));

                    cmd.ExecuteScalar();
                }
            }
        }

        /************************************************************************************
        * Find all instructors in the database.Include each instructor's cohort.
        ************************************************************************************/
        public List<Instructor> GetInstructorsWithCohort()
        {
            var instructors = new List<Instructor>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, i.FirstName, i.LastName, c.CohortNum
                                        FROM Instructor i
                                        JOIN CohortInstructors ci
                                        ON ci.InstructorId = i.Id
                                        JOIN Cohort c
                                        ON c.Id = ci.CohortId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        var FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        var LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        var CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum"));

                        Instructor instructor = new Instructor
                        {
                            Id = idValue,
                            FirstName = FirstName,
                            LastName = LastName,
                            cohort = new Cohort { CohortNum = CohortNum}
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();

                    return instructors;
                }
            }
        }

        //Insert a new instructor into the database.Assign the instructor to an existing cohort.

        //Assign an existing exercise to an existing student.

    }
}
