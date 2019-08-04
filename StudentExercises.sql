-- Create tables from each entity in the Student Exercises ERD.
IF (NOT EXISTS (SELECT TOP 1 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'DBO' 
                 AND  TABLE_NAME = 'Cohort'))
BEGIN
CREATE TABLE Cohort (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    IsDayTime BIT NULL DEFAULT 1,
    CohortNum INTEGER NOT NULL
);
END

IF (NOT EXISTS (SELECT TOP 1 1
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'DBO' 
                 AND  TABLE_NAME = 'Exercise'))
BEGIN
CREATE TABLE Exercise (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    ExName VARCHAR(55) NOT NULL,
    ExLanguage VARCHAR(55) NOT NULL
);
END

IF (NOT EXISTS (SELECT TOP 1 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'DBO' 
                 AND  TABLE_NAME = 'Instructor'))
BEGIN
CREATE TABLE Instructor (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(55) NOT NULL,
    LastName VARCHAR(55) NOT NULL,
    SlackHandle VARCHAR(55) NOT NULL
);
END

IF (NOT EXISTS (SELECT TOP 1 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'DBO' 
                 AND  TABLE_NAME = 'Student'))
BEGIN
CREATE TABLE Student (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    FirstName VARCHAR(55) NOT NULL,
    LastName VARCHAR(55) NOT NULL,
    SlackHandle VARCHAR(55) NOT NULL
);
END

IF (NOT EXISTS (SELECT TOP 1 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'DBO' 
                 AND  TABLE_NAME = 'CohortInstructors'))
BEGIN
CREATE TABLE CohortInstructors (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    CohortId INTEGER FOREIGN KEY REFERENCES Cohort(Id),
    InstructorId INTEGER FOREIGN KEY REFERENCES Instructor(Id)
);
END

IF (NOT EXISTS (SELECT TOP 1 1
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'DBO' 
                 AND  TABLE_NAME = 'CohortStudents'))
BEGIN
CREATE TABLE CohortStudents (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    CohortId INTEGER FOREIGN KEY REFERENCES Cohort(Id),
    StudentId INTEGER FOREIGN KEY REFERENCES Student(Id)
);
END

IF (NOT EXISTS (SELECT TOP 1 1
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'DBO' 
                 AND  TABLE_NAME = 'StudentExercises'))
BEGIN
CREATE TABLE StudentExercises (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    ExerciseId INTEGER FOREIGN KEY REFERENCES Exercise(Id),
    StudentId INTEGER FOREIGN KEY REFERENCES Student(Id)
);
END

-- Populate each table with data. 

--You should have 2-3 cohorts, 
IF (NOT EXISTS (SELECT TOP 1 1
                    FROM Cohort
                    WHERE CohortNum = 31))
BEGIN
INSERT INTO Cohort (IsDayTime, CohortNum) VALUES (1, 31);
END

IF (NOT EXISTS (SELECT TOP 1 1
                    FROM Cohort
                    WHERE CohortNum = 32))
BEGIN
INSERT INTO Cohort (IsDayTime, CohortNum) VALUES (1, 32);
END

IF (NOT EXISTS (SELECT TOP 1 1
                    FROM Cohort
                    WHERE CohortNum = 9))
BEGIN
INSERT INTO Cohort (IsDayTime, CohortNum) VALUES (0, 9);
END

-- 5-10 students, 
IF (NOT EXISTS (SELECT TOP 1 1
                    FROM Student
                    WHERE FirstName = 'Eliot'))
BEGIN
INSERT INTO Student (FirstName, LastName, SlackHandle) VALUES ('Eliot', 'Clarke', 'Elllliiooootttt');
INSERT INTO Student (FirstName, LastName, SlackHandle) VALUES ('Emily', 'Loggins', 'VeetBoopBeep');
INSERT INTO Student (FirstName, LastName, SlackHandle) VALUES ('Addam', 'Joor', 'Maddarooj');
INSERT INTO Student (FirstName, LastName, SlackHandle) VALUES ('Heather', 'Cleland', 'Clllleeeeee');
INSERT INTO Student (FirstName, LastName, SlackHandle) VALUES ('Shelley', 'Arnold', 'SnailFingers');
INSERT INTO Student (FirstName, LastName, SlackHandle) VALUES ('Sean', 'Gavin', 'GB2019');
INSERT INTO Student (FirstName, LastName, SlackHandle) VALUES ('Nate', 'Mate', 'NatesaMate');
END

-- 4-8 instructors, 
IF (NOT EXISTS (SELECT TOP 1 1
                    FROM Instructor
                    WHERE FirstName = 'Adam'))
BEGIN
INSERT INTO Instructor (FirstName, LastName, SlackHandle) VALUES ('Adam', 'Hat', 'AdamsHat');
INSERT INTO Instructor (FirstName, LastName, SlackHandle) VALUES ('Jisie', 'David', 'jisie');
INSERT INTO Instructor (FirstName, LastName, SlackHandle) VALUES ('Kristen', 'Noris', 'kristin.norris');
END

-- 2-5 exercises and 
IF (NOT EXISTS (SELECT TOP 1 1
                    FROM Exercise
                    WHERE ExLanguage = 'JavaScript'))
BEGIN
INSERT INTO Exercise (ExLanguage, ExName) VALUES ('JavaScript', 'Daily Journal');
INSERT INTO Exercise (ExLanguage, ExName) VALUES ('CSS', 'Jim Cooper');
INSERT INTO Exercise (ExLanguage, ExName) VALUES ('HTML', 'Celebrity Tribute');
INSERT INTO Exercise (ExLanguage, ExName) VALUES ('React', 'Nutshell 2.0');
END

-- each student should be assigned 1-2 exercises
IF (NOT EXISTS (SELECT TOP 1 1
                        FROM StudentExercises
                        WHERE ExerciseId = 1 AND StudentId = 1))
BEGIN
INSERT INTO StudentExercises (ExerciseId, StudentId) VALUES (1, 1)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM StudentExercises
                        WHERE ExerciseId = 1 AND StudentId = 2))
BEGIN
INSERT INTO StudentExercises (ExerciseId, StudentId) VALUES (1, 2)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM StudentExercises
                        WHERE ExerciseId = 2 AND StudentId = 1))
BEGIN
INSERT INTO StudentExercises (ExerciseId, StudentId) VALUES (2, 1)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM StudentExercises
                        WHERE ExerciseId = 2 AND StudentId = 2))
BEGIN
INSERT INTO StudentExercises (ExerciseId, StudentId) VALUES (2, 2)
END

-- tie instructors to cohorts

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortInstructors
                        WHERE CohortId = 2 AND InstructorId = 1))
BEGIN
INSERT INTO CohortInstructors (CohortId, InstructorId) VALUES (1, 1)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortInstructors
                        WHERE CohortId = 2 AND InstructorId = 2))
BEGIN
INSERT INTO CohortInstructors (CohortId, InstructorId) VALUES (1, 2)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortInstructors
                        WHERE CohortId = 2 AND InstructorId = 3))
BEGIN
INSERT INTO CohortInstructors (CohortId, InstructorId) VALUES (1, 3)
END

-- tie students to cohorts

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortStudents
                        WHERE CohortId = 2 AND StudentId = 1))
BEGIN
INSERT INTO CohortStudents (CohortId, StudentId) VALUES (1, 1)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortStudents
                        WHERE CohortId = 2 AND StudentId = 2))
BEGIN
INSERT INTO CohortStudents (CohortId, StudentId) VALUES (1, 2)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortStudents
                        WHERE CohortId = 2 AND StudentId = 3))
BEGIN
INSERT INTO CohortStudents (CohortId, StudentId) VALUES (1, 3)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortStudents
                        WHERE CohortId = 2 AND StudentId = 4))
BEGIN
INSERT INTO CohortStudents (CohortId, StudentId) VALUES (1, 4)
END

IF (NOT EXISTS (SELECT TOP 1 1
                        FROM CohortStudents
                        WHERE CohortId = 2 AND StudentId = 5))
BEGIN
INSERT INTO CohortStudents (CohortId, StudentId) VALUES (1, 5)
END