using System;
using System.Collections.Generic;
using System.Data;
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
                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, c.CohortNum, i.SlackHandle
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
                        var slackHanlde = reader.GetString(reader.GetOrdinal("SlackHandle"));

                        Instructor instructor = new Instructor
                        {
                            Id = idValue,
                            FirstName = FirstName,
                            LastName = LastName,
                            SlackHandle = slackHanlde,
                            cohort = new Cohort { CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum")), CohortName = "Cohort " + reader.GetInt32(reader.GetOrdinal("CohortNum")) }
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();

                    return instructors;
                }
            }
        }

        /************************************************************************************
        * Insert a new instructor into the database.
        * Assign the instructor to an existing cohort.
        ************************************************************************************/
        public void InsertInstructorWithAssign(string firstName, string lastName, string slackHandle, int cohortNum)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        DECLARE @InstructorTemp TABLE (Id int);
                                        
                                        INSERT INTO Instructor (FirstName, LastName, SlackHandle)
                                        OUTPUT INSERTED.Id INTO @InstructorTemp(Id)
                                        VALUES (@FirstName, @LastName, @SlackHandle)

                                        SELECT TOP 1 @ID = Id FROM @InstructorTemp";

                    SqlParameter outputParam = cmd.Parameters.Add("@ID", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(new SqlParameter("@FirstName", firstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", lastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", slackHandle));

                    cmd.ExecuteNonQuery();

                    var newInstructorId = (int)outputParam.Value;

                    cmd.CommandText = @"INSERT INTO CohortInstructors (CohortId, InstructorId)
                                        SELECT c.Id, @InstructorId
                                        FROM Cohort c
                                        WHERE c.CohortNum = @CohortNum";

                    cmd.Parameters.Add(new SqlParameter("@InstructorId", newInstructorId));
                    cmd.Parameters.Add(new SqlParameter("@CohortNum", cohortNum));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /************************************************************************************
        * Assign an existing exercise to an existing student.
        ************************************************************************************/
        public void AddExerciseToStudent(string firstName, string lastName, string slackHandle, string exerciseName)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DECLARE @ExerciseId int = 0;
                                        SELECT TOP 1 @ExerciseId = e.Id FROM Exercise e WHERE e.ExName = @exerciseName
                                        IF @ExerciseId > 0
                                        BEGIN
                                        INSERT INTO StudentExercises (ExerciseId, StudentId)
                                        SELECT @ExerciseId, s.Id FROM Student s
                                        WHERE s.FirstName = @FirstName AND s.LastName = @LastName AND s.SlackHandle = @SlackHandle
                                        END";

                    cmd.Parameters.Add(new SqlParameter("@FirstName", firstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", lastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", slackHandle));
                    cmd.Parameters.Add(new SqlParameter("@exerciseName", exerciseName));

                    cmd.ExecuteNonQuery();

                }
            }
        }


        /************************************************************************************
        * Additional Methods
        ************************************************************************************/
        public List<Student> GetAllStudent()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, c.CohortNum
                                        FROM Student s
                                        JOIN CohortStudents cs
                                        ON cs.StudentId = s.Id
                                        JOIN Cohort c
                                        ON cs.CohortId = c.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        var student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            cohort = new Cohort { CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum")), CohortName = "Cohort " + reader.GetInt32(reader.GetOrdinal("CohortNum")) },
                            exerciseList = GetAllExercisesByStudentId(reader.GetInt32(reader.GetOrdinal("Id")))
                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return students;
                }
            }
        }

        public Student GetOneStudent(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT
                                    Id, FirstName, LastName, SlackHandle
                                FROM Student
                                WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                        };
                    }

                    reader.Close();

                    return student;
                }
            }
        }

        public Student AddStudent(Student newStudent)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, SlackHandle)
                                                OUTPUT INSERTED.Id
                                                VALUES (@firstName, @lastName, @slackHandle)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", newStudent.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newStudent.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newStudent.SlackHandle));

                    int newId = (int)cmd.ExecuteScalar();
                    newStudent.Id = newId;
                    return newStudent;
                }
            }
        }

        public Student UpdateStudent(int id, Student student) 
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student
                                                    SET FirstName = @firstName,
                                                        LastName = @lastName,
                                                        SlackHandle = @slackHandle
                                                    WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!StudentExists(id))
                {
                    throw;
                }
            }

            return student;
        }

        public void DeleteStudent(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Student WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!StudentExists(id))
                {
                   throw;
                }
            }
        }

        public List<Exercise> GetAllExercisesByStudentId(int studentId)
        {
            var exercises = new List<Exercise>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.ExName, e.ExLanguage FROM Exercise e
                                        JOIN StudentExercises se
                                         ON se.ExerciseId = e.Id
                                        WHERE se.StudentId = @StudentId";

                    cmd.Parameters.Add(new SqlParameter("@StudentId", studentId));


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

        private bool StudentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT Id, FirstName, LastName, SlackHandle
                                FROM Student
                                WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
