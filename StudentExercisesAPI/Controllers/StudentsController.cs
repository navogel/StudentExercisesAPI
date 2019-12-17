using System;
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
                    cmd.CommandText = @"SELECT c.Id AS CohortId, c.Name AS CohortName, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Id AS ExerciseId, e.Name, e.Language FROM Student s
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
                        //create bool for if student is added
                        var studentAlreadyAdded = students.FirstOrDefault(s => s.Id == studentId);
                       //create bool for if there is an exercise in row
                        var hasExercise = !reader.IsDBNull(reader.GetOrdinal("ExerciseId"));
                        //if for new student
                        if (studentAlreadyAdded == null)
                        {

                            Student student = new Student
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
                            students.Add(student);



                            var exerciseId = reader.GetInt32(reader.GetOrdinal("ExerciseId"));
                            var exerciseAlreadyAdded = student.StudentsExercises.FirstOrDefault(se => se.Id == exerciseId);
                            
                            //look for having an exercise and it has not been added
                            if (hasExercise && exerciseAlreadyAdded == null && include == "exercises")
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
                        {
                            var exerciseId = reader.GetInt32(reader.GetOrdinal("ExerciseId"));
                            var exerciseAlreadyAdded = studentAlreadyAdded.StudentsExercises.FirstOrDefault(se => se.Id == exerciseId);

                            if (hasExercise && exerciseAlreadyAdded == null && include == "exercises")
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


        [HttpGet("{id}", Name = "GetStudent")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id AS CohortId, c.Name AS CohortName, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Id AS ExerciseId, e.Name, e.Language FROM Student s
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

        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] Student student)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"INSERT INTO Student (Title, BeanType)
        //                                OUTPUT INSERTED.Id
        //                                VALUES (@title, @beanType)";
        //            cmd.Parameters.Add(new SqlParameter("@title", student.Title));
        //            cmd.Parameters.Add(new SqlParameter("@beanType", student.BeanType));

        //            int newId = (int)cmd.ExecuteScalar();
        //            student.Id = newId;
        //            return CreatedAtRoute("GetStudent", new { id = newId }, student);
        //        }
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Student student)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Student
        //                                    SET Title = @title,
        //                                        BeanType = @beanType
        //                                    WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@title", student.Title));
        //                cmd.Parameters.Add(new SqlParameter("@beanType", student.BeanType));
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                return BadRequest($"No student with Id of {id}");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!StudentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"DELETE FROM Student WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!StudentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //private bool StudentExists(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                SELECT Id, Title, BeanType
        //                FROM Student
        //                WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            SqlDataReader reader = cmd.ExecuteReader();
        //            return reader.Read();
        //        }
        //    }
        //}
    }
}
