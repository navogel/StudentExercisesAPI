﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using StudentExercisesAPI.Models;
using Microsoft.AspNetCore.Http;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string firstName, [FromQuery]string lastName, [FromQuery] string slackHandle, [FromQuery] string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, c.Id AS CohortId, c.Name AS CohortName,  e.Id AS ExerciseId, e.Name, e.Language FROM Student s
                                        LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                        LEFT JOIN Exercise e ON e.Id = se.ExerciseId
                                        LEFT JOIN Cohort c ON s.CohortId = c.Id


                                        WHERE 1=1";

                    if (firstName != null)
                    {
                        cmd.CommandText += " AND s.FirstName LIKE @FirstName";
                        cmd.Parameters.Add(new SqlParameter(@"FirstName", firstName));
                    }

                    if (lastName != null)
                    {
                        cmd.CommandText += " AND s.LastName LIKE @LastName";
                        cmd.Parameters.Add(new SqlParameter(@"LastName", "%" + lastName + "%"));
                    }

                    if (slackHandle != null)
                    {
                        cmd.CommandText += " AND s.SlackHandle LIKE @SlackHandle";
                        cmd.Parameters.Add(new SqlParameter(@"SlackHandle", "%" + slackHandle + "%"));
                    }



                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Student> students = new List<Student>();
                    

                    while (reader.Read())
                    {
                        //create student ID
                        var studentId = reader.GetInt32(reader.GetOrdinal("Id"));
                        //search to see if student is already added
                        var studentAlreadyAdded = students.FirstOrDefault(s => s.Id == studentId);
                       //create bool for if there is an exercise in row
                        var hasExercise = !reader.IsDBNull(reader.GetOrdinal("ExerciseId"));
                        //if statement for adding new student, null means they were NOT found, let's add them!
                        if (studentAlreadyAdded == null)
                        {

                            Student student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                Cohort = new Cohort()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    StudentsInCohort = new List<Student>(),
                                    InstructorsInCohort = new List<Instructor>()
                                }

                            };
                            students.Add(student);

                            //If row has an exercise AND the query param "include" = exercises, then add it to the exercise list
                            if (hasExercise && include == "exercises")
                            {
                                student.StudentsExercises.Add(new Exercise()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                });

                            }
                        }
                        else
                        //Student was already added!  Lets check to see if there are exercises to add.
                        {
                            

                            if (hasExercise && include == "exercises")
                            {
                                studentAlreadyAdded.StudentsExercises.Add(new Exercise()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                });

                            }
                        }
                    }
                    reader.Close();
                    //from controllerbase interface - returns official json result with 200 status code
                    return Ok(students);
                }
            }
        }

        //temp test for shortening a get
        //[HttpGet]
        //public async Task<IActionResult> Get([FromQuery] string include)
        //{
        //    var students = await GetAllStudents();
        //    return Ok(students);
        //}



        [HttpGet("{id}", Name = "GetStudent")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, c.Id AS CohortId, c.Name AS CohortName,  e.Id AS ExerciseId, e.Name, e.Language FROM Student s
                                        LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                        LEFT JOIN Exercise e ON e.Id = se.ExerciseId
                                        LEFT JOIN Cohort c ON s.CohortId = c.Id


                                        WHERE s.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Student student = null;

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            StudentsExercises = new List<Exercise>(),
                            Cohort = new Cohort()
                            {
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                StudentsInCohort = new List<Student>(),
                                InstructorsInCohort = new List<Instructor>()
                            }

                        };
                    }
                    reader.Close();

                    if (student == null)
                    {
                        return NotFound($"No Student found wit the ID of {id}");
                    }
                    return Ok(student);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, SlackHandle, CohortId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@FirstName, @LastName, @SlackHandle, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));

                    int newId = (int) await cmd.ExecuteScalarAsync();
                    student.Id = newId;
                    return CreatedAtRoute("GetStudent", new { id = newId }, student);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student
                                            SET FirstName = @FirstName,
                                                LastName = @LastName,
                                                SlackHandle = @SlackHandle,
                                                CohortId = @CohortId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@SlackHandle", student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        return BadRequest($"No student with Id of {id}");
                    }
                }
            }
            catch (Exception)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
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

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
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
                        SELECT Id, FirstName, LastName, CohortId
                        FROM Student
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

        //private async Task<List<Student>> GetStudentsWithExercises()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                    SELECT s.id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, e.[Name], e.Id as ExerciseId, e.Language
        //                    FROM Student s
        //                    LEFT JOIN StudentExercise se ON s.Id = se.StudentId
        //                    LEFT JOIN Exercise e ON se.ExerciseId = e.Id
        //                    ";

        //            SqlDataReader reader = await cmd.ExecuteReaderAsync();

        //            var students = new List<Student>();

        //            while (reader.Read())
        //            {
        //                students.Add(new Student
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
        //                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
        //                });

        //            }
        //            reader.Close();
        //            return students;
        //        }

        //    }
        //}

        //private async Task<List<Student>> GetAllStudents()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                    SELECT id, FirstName, LastName, SlackHandle, CohortId
        //                    FROM Student
        //                    ";

        //            SqlDataReader reader = await cmd.ExecuteReaderAsync();

        //            var students = new List<Student>();

        //            while (reader.Read())
        //            {
        //                students.Add(new Student
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
        //                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
        //                });

        //            }
        //            reader.Close();
        //            return students;
        //        }

        //    }
        //}
    }
}
