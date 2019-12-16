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
        public async Task<IActionResult> Get([FromQuery]int? cohortId, [FromQuery]string lastName)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id AS CohortId, c.Name AS CohortName, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Name, e.Language, i.Id AS InstructorId, i.[FirstName] as InstructorFirst, i.LastName as InstructorLast FROM Student s
                                        LEFT JOIN Instructor i ON s.CohortId = i.CohortId
                                        LEFT JOIN StudentExercise se ON se.StudentId = s.Id
                                        LEFT JOIN Exercise e ON e.Id = se.ExerciseId
                                        LEFT JOIN Cohort c ON s.CohortId = c.Id

                                        WHERE 1=1
                                        GROUP BY c.Id, c.Name, s.Id, s.FirstName, s.LastName, s.SlackHandle, e.Name, e.Language, i.Id, i.FirstName, i.LastName;";
                                           
                    //if (cohortId != null)
                    //{
                    //    cmd.CommandText += " AND CohortId = @cohortId";
                    //    cmd.Parameters.Add(new SqlParameter(@"CohortId", cohortId));
                    //}

                    //if (lastName != null)
                    //{
                    //    cmd.CommandText += " AND LastName LIKE @LastName";
                    //    cmd.Parameters.Add(new SqlParameter(@"LastName", "%" + lastName + "%"));
                    //}
                    
                    

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Student> students = new List<Student>();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        var CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                        var cohortAlreadyAdded = cohorts.FirstOrDefault(c => c.Id == cohortId);

                        
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
                        };

                        students.Add(student);
                    }
                    reader.Close();
                    //from controllerbase interface - returns official json result with 200 status code
                    return Ok(students);
                }
            }
        }
        //working 
        //[HttpGet]
        //public async Task<IActionResult> Get([FromQuery]int? cohortId, [FromQuery]string lastName)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle, CohortId FROM Student
        //                                WHERE 1=1";

        //            if (cohortId != null)
        //            {
        //                cmd.CommandText += " AND CohortId = @cohortId";
        //                cmd.Parameters.Add(new SqlParameter(@"CohortId", cohortId));
        //            }

        //            if (lastName != null)
        //            {
        //                cmd.CommandText += " AND LastName LIKE @LastName";
        //                cmd.Parameters.Add(new SqlParameter(@"LastName", "%" + lastName + "%"));
        //            }



        //            SqlDataReader reader = cmd.ExecuteReader();

        //            List<Student> students = new List<Student>();

        //            while (reader.Read())
        //            {
        //                Student student = new Student
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
        //                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"))
        //                };

        //                students.Add(student);
        //            }
        //            reader.Close();
        //            //from controllerbase interface - returns official json result with 200 status code
        //            return Ok(students);
        //        }
        //    }
        //}

        //[HttpGet("{id}", Name = "GetStudent")]
        //public async Task<IActionResult> Get([FromRoute] int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                SELECT
        //                    Id, Title, BeanType
        //                FROM Student
        //                WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            Student student = null;

        //            if (reader.Read())
        //            {
        //                student = new Student
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    Title = reader.GetString(reader.GetOrdinal("Title")),
        //                    BeanType = reader.GetString(reader.GetOrdinal("BeanType"))
        //                };
        //            }
        //            reader.Close();

        //            if (student == null)
        //            {
        //                return NotFound($"No Student found wit the ID of {id}");
        //            }
        //            return Ok(student);
        //        }
        //    }
        //}

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
