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
        * Exercise JSON response should have all currently assigned students if the
          include=students query string parameter is there.
        ************************************************************************************/
        public List<Exercise> GetAllExercises(string q, string _include, string active)
        {
            var studentList = new List<Student>();

            using (SqlConnection conn = Connection)
            {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    var filter = "";

                    if (!string.IsNullOrEmpty(q))
                    {
                        filter = @" WHERE e.ExName
                                        LIKE '%' + @filter + '%' 
                                        OR e.ExLanguage
                                        LIKE '%' + @filter + '%' ";
                    }
                    cmd.CommandText = @"SELECT Id, ExName, ExLanguage
                                        FROM Exercise" + filter;

                    if (!string.IsNullOrEmpty(q))
                    {
                        cmd.Parameters.Add(new SqlParameter("@filter", q));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Exercise> exercises = new List<Exercise>();

                    while (reader.Read())
                    {
                        if (_include == "student")
                            studentList = GetAllStudentsByExerciseId(reader.GetInt32(reader.GetOrdinal("Id")));

                        int idColumnPosition = reader.GetOrdinal("Id");

                        int idValue = reader.GetInt32(idColumnPosition);

                        var exName = reader.GetString(reader.GetOrdinal("ExName"));
                        var exLanguage = reader.GetString(reader.GetOrdinal("ExLanguage"));

                        Exercise exercise = new Exercise
                        {
                            Id = idValue,
                            ExName = exName,
                            ExLanguage = exLanguage,
                            studentList = studentList
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
        * Provide support for each resource (Instructor, Student, Cohort, Exercise) and the q query string parameter.
          If it is provided, your SQL should search relevant property for a match, search all properties of the resource for a match.
            FirstName, LastName, and SlackHandle for instructors and students.
            Name and Language for exercises.
            Name for cohorts.
        ************************************************************************************/
        public List<Instructor> GetInstructorsWithCohort(string q, string _include, string active)
        {
            var instructors = new List<Instructor>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    var filter = "";

                    if(!string.IsNullOrEmpty(q))
                    {
                        filter = @" WHERE i.FirstName
                                    LIKE '%' + @filter + '%' 
                                    OR i.LastName
                                    LIKE '%' + @filter + '%' 
                                    OR i.SlackHandle
                                    LIKE '%' + @filter + '%' ";
                    }

                    cmd.CommandText = @"SELECT i.Id, i.FirstName, i.LastName, c.CohortNum, i.SlackHandle
                                        FROM Instructor i
                                        JOIN CohortInstructors ci
                                        ON ci.InstructorId = i.Id
                                        JOIN Cohort c
                                        ON c.Id = ci.CohortId" + filter;

                    if (!string.IsNullOrEmpty(q))
                    {
                        cmd.Parameters.Add(new SqlParameter("@filter", q));
                    }

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
        * Student JSON response should have all exercises that are assigned to them if the
        include=exercise query string parameter is there.
        ************************************************************************************/
        public List<Student> GetAllStudents(string q, string _include, string active)
        {
            var exerciseList = new List<Exercise>();

            using (SqlConnection conn = Connection)
            {
                var filter = "";

                if (!string.IsNullOrEmpty(q))
                {
                    filter = @" WHERE s.FirstName
                                    LIKE '%' + @filter + '%' 
                                    OR s.LastName
                                    LIKE '%' + @filter + '%' 
                                    OR s.SlackHandle
                                    LIKE '%' + @filter + '%' ";
                }
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, c.CohortNum
                                        FROM Student s
                                        JOIN CohortStudents cs
                                        ON cs.StudentId = s.Id
                                        JOIN Cohort c
                                        ON cs.CohortId = c.Id" + filter;

                    if (!string.IsNullOrEmpty(q))
                    {
                        cmd.Parameters.Add(new SqlParameter("@filter", q));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        if (_include == "exercise")
                            exerciseList = GetAllExercisesByStudentId(reader.GetInt32(reader.GetOrdinal("Id")));

                        var student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            cohort = new Cohort { CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum")), CohortName = "Cohort " + reader.GetInt32(reader.GetOrdinal("CohortNum")) },
                            exerciseList = exerciseList
                        };

                        students.Add(student);
                    }
                    reader.Close();

                    return students;
                }
            }
        }

        /************************************************************************************
        * Additional Methods
        ************************************************************************************/
        public Student GetOneStudent(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle
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
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
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
                    cmd.CommandText = @"DECLARE @TempStudent TABLE (Id int)
                                        INSERT INTO Student (FirstName, LastName, SlackHandle)
                                        OUTPUT INSERTED.Id INTO @TempStudent
                                        VALUES (@firstName, @lastName, @slackHandle)

                                        SELECT top 1 @OutputId = Id FROM @TempStudent";

                    SqlParameter outPutId = new SqlParameter("@OutputId", SqlDbType.Int);
                    outPutId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outPutId);

                    cmd.Parameters.Add(new SqlParameter("@firstName", newStudent.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", newStudent.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", newStudent.SlackHandle));

                    cmd.ExecuteScalar();
                    newStudent.Id = (int)outPutId.Value;
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

        public List<Student> GetAllStudentsByExerciseId(int exerciseId)
        {
            var students = new List<Student>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName
                                        FROM Student s
                                        JOIN StudentExercises se
                                        ON se.StudentId = s.Id
                                        WHERE se.ExerciseId = @ExerciseId";

                    cmd.Parameters.Add(new SqlParameter("@ExerciseId", exerciseId));


                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");

                        int idValue = reader.GetInt32(idColumnPosition);

                        var FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        var LastName = reader.GetString(reader.GetOrdinal("LastName"));

                        Student student = new Student
                        {
                            Id = idValue,
                            FirstName = FirstName,
                            LastName = LastName
                        };

                        students.Add(student);
                    }

                    reader.Close();

                    return students;
                }
            }
        }


        /************************************************************************************
        * CRUD for Cohorts
        ************************************************************************************/
        public List<Cohort> GetAllCohorts(string q, string _include, string active)
        {
            var cohortList = new List<Cohort>();

            using (SqlConnection conn = Connection)
            {
                var filter = "";

                if (!string.IsNullOrEmpty(q))
                {
                    filter = @" WHERE CohortNum
                                    LIKE '%' + @filter + '%' ";
                }
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CohortNum
                                        FROM Cohort" + filter;

                    if (!string.IsNullOrEmpty(q))
                    {
                        cmd.Parameters.Add(new SqlParameter("@filter", q));
                    }
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        var cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum")),
                            CohortName = "Cohort " + reader.GetInt32(reader.GetOrdinal("CohortNum"))
                        };

                        cohortList.Add(cohort);
                    }
                    reader.Close();

                    return cohortList;
                }
            }

        }

        public Cohort GetOneCohort(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, IsDayTime, CohortNum
                                        FROM Cohort
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            IsDayTime = reader.GetBoolean(reader.GetOrdinal("IsDayTime")),
                            CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum"))
                        };
                    }

                    reader.Close();

                    return cohort;
                }
            }
        }

        public Cohort AddCohort(Cohort newCohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DECLARE @TempCohort TABLE (Id int)
                                        INSERT INTO Cohort (IsDayTime, CohortNum)
                                        OUTPUT INSERTED.Id INTO @TempCohort
                                        VALUES (@isDayTime, @cohortNum)

                                        SELECT top 1 @OutputId = Id FROM @TempCohort";

                    SqlParameter outPutId = new SqlParameter("@OutputId", SqlDbType.Int);
                    outPutId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outPutId);

                    cmd.Parameters.Add(new SqlParameter("@isDayTime", newCohort.IsDayTime));
                    cmd.Parameters.Add(new SqlParameter("@cohortNum", newCohort.CohortNum));

                    cmd.ExecuteScalar();
                    newCohort.Id = (int)outPutId.Value;
                    return newCohort;
                }
            }
        }

        public Cohort UpdateCohort(int id, Cohort cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Cohort
                                            SET IsDayTime = @isDayTime,
                                                CohortNum = @cohortNum
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@isDayTime", cohort.IsDayTime));
                        cmd.Parameters.Add(new SqlParameter("@cohortNum", cohort.CohortNum));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CohortExists(id))
                {
                    throw;
                }
            }

            return cohort;
        }

        public void DeleteCohort(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Cohort WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                if (!CohortExists(id))
                {
                    throw;
                }
            }
        }

        /************************************************************************************
        * CRUD for Instructors
        ************************************************************************************/
        public Instructor GetOneInstructor(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle
                                        FROM Instructor
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
                        };
                    }

                    reader.Close();

                    return instructor;
                }
            }
        }

        public Instructor UpdateInstructor(int id, Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Instructor
                                            SET FirstName = @firstName,
                                                LastName = @lastName,
                                                SlackHandle = @slackHandle
                                             WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@firstName", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!InstructorExists(id))
                {
                    throw;
                }
            }

            return instructor;
        }

        public void DeleteInstructor(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Instructor WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                if (!InstructorExists(id))
                {
                    throw;
                }
            }
        }

        /************************************************************************************
        * Add the following to your program:
          Find all the students in the database.Include each student's cohort
          AND each student's list of exercises.
        ************************************************************************************/
        public List<Student> GetStudentsWithCohortExercise()
        {
            var students = new List<Student>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, c.CohortNum, e.ExName
                                        FROM Student s
                                        JOIN CohortInstructors ci
                                        ON ci.InstructorId = s.Id
                                        JOIN Cohort c
                                        ON c.Id = ci.CohortId
                                        JOIN StudentExercises se
                                        ON s.Id = se.StudentId
                                        JOIN Exercise e
                                        ON e.Id = se.ExerciseId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        var FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        var LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        var CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum"));
                        var SlackHanlde = reader.GetString(reader.GetOrdinal("SlackHandle"));
                        var ExName = reader.GetString(reader.GetOrdinal("ExName"));

                        Student student = new Student
                        {
                            Id = idValue,
                            FirstName = FirstName,
                            LastName = LastName,
                            SlackHandle = SlackHanlde,
                            cohort = new Cohort { CohortNum = reader.GetInt32(reader.GetOrdinal("CohortNum")), CohortName = "Cohort " + reader.GetInt32(reader.GetOrdinal("CohortNum")) },
							exerciseList = GetAllExercisesByStudentId(idValue)
					};

                        students.Add(student);
                    }

                    reader.Close();

                    return students;
                }
            }
        }

        /************************************************************************************
        * Write a method in the Repository class that accepts an Exercise and
          a Cohort and assigns that exercise to each student in the cohort
          IF and ONLY IF the student has not already been assigned the exercise.
        ************************************************************************************/
        public void AssignCohortExercises(int exerciseId, int cohortId)
		{
			using (SqlConnection conn = Connection)
			{
				conn.Open();

				using (SqlCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = @"INSERT INTO StudentExercises
                                        (StudentId, ExerciseId)
                                        SELECT s.Id, @exerciseId
                                        FROM Student s 
                                        JOIN CohortStudents cs
                                        ON cs.StudentId = s.Id
                                        WHERE cs.CohortId = @cohortId 
                                        AND s.Id NOT IN (SELECT StudentExercises.StudentId
                                        FROM StudentExercises
                                        WHERE StudentExercises.ExerciseId = @exerciseId)";

					cmd.Parameters.Add(new SqlParameter("@exerciseId", exerciseId));
					cmd.Parameters.Add(new SqlParameter("@cohortId", cohortId));

					cmd.ExecuteScalar();
				}
			}
		}

        /************************************************************************************
        * Private Methods
        ************************************************************************************/
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

        private bool CohortExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT Id, IsDayTime, CohortNum
                                FROM Cohort
                                WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

        private bool InstructorExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT Id, FirstName, LastName, SlackHandle
                                FROM Instructor
                                WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}
