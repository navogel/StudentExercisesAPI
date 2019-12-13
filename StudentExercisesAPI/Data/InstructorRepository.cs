using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using StudentExercisesContinued.models;

namespace StudentExercisesAPI.Data
{
    /// <summary>
    ///  An object to contain all database interactions.
    /// </summary>
    public class InstructorRepository
    {
        /// <summary>
        ///  Represents a connection to the database.
        ///   This is a "tunnel" to connect the application to the database.
        ///   All communication between the application and database passes through this connection.
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                // This is "address" of the database
                string _connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=SE;Integrated Security=True";
                return new SqlConnection(_connectionString);
            }
        }

        /// <summary>
        ///  Returns a list of all instructors in the database
        /// </summary>
        public List<Instructor> GetAllInstructors()
        {

            using (SqlConnection conn = Connection)
            {

                conn.Open();


                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, SlackHandle, CohortId, Specialty FROM Instructor";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {

                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();

                    return instructors;
                }
            }
        }

        /// <summary>
        ///  Returns a single instructor with the given id.
        /// </summary>
        public Instructor GetInstructorById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName, SlackHandle, CohortId, Specialty FROM Instructor WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = id,
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))

                        };
                    }

                    reader.Close();

                    return instructor;
                }
            }
        }

        /// <summary>
        ///  Add a new instructor to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void AddInstructor(Instructor instructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = "INSERT INTO Instructor (FirstName, LastName, SlackHandle, CohortId, Specialty) OUTPUT INSERTED.Id Values (@FirstName, @LastName, @SlackHandle, @CohortID)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", instructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", instructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", instructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", instructor.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@Specialty", instructor.Specialty));


                    int id = (int)cmd.ExecuteScalar();

                    instructor.Id = id;


                }
            }

        }

        /// <summary>
        ///  Updates the instructor with the given id
        /// </summary>
        public void UpdateInstructor(int id, Instructor instructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Instructor
                                     SET FirstName = @FirstName, LastName = @LastName, SlackHandle = @SlackHandle, CohortId = @CohortId, Specialty = @Specialty
                                     WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", instructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", instructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", instructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", instructor.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@Specialty", instructor.Specialty));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///  Delete the instructor with the given id
        /// </summary>
        public void DeleteInstructor(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Instructor WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}


