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

namespace Student.Controllers
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
        public async Task<IActionResult> Get(int? cohortId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Title, BeanType FROM Student";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            BeanType = reader.GetString(reader.GetOrdinal("BeanType"))
                        };

                        students.Add(student);
                    }
                    reader.Close();
                    //from controllerbase interface - returns official json result with 200 status code
                    return Ok(students);
                }
            }
        }

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
