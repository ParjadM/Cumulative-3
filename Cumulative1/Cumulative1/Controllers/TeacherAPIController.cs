using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Cumulative1.Models;
using System;
using System.Collections.Generic;
using Cumulative1.Model;
using System.Diagnostics;

namespace Cumulative1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        // Dependency injection of database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns detailed information about a specific teacher by ID.
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/1 
        /// </example>
        /// <param name="id">The ID of the teacher to retrieve.</param>
        /// <returns>
        /// A teacher object containing detailed information.
        /// </returns>
        [HttpGet]
        [Route("FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            // Initialize an empty Teacher object
            Teacher selectedTeacher = null;

            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText= "SELECT * FROM teachers WHERE teacherid = @id";
                command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader resultSet = command.ExecuteReader())
                {
                    if (resultSet.Read())
                    {
                        selectedTeacher = new Teacher
                        {
                            Id = Convert.ToInt32(resultSet["teacherid"]),
                            Name = $"{resultSet["teacherfname"]} {resultSet["teacherlname"]}",
                            EmployeeNumber = resultSet["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(resultSet["hiredate"])
                        };
                    }
                }
            }

            // Return the selected teacher or null if not found
            return selectedTeacher;
        }

        /// <summary>
        /// Returns a list of all teachers in the system.
        /// </summary>
        /// <example>
        /// GET api/Teacher/ListTeachers 
        /// </example>
        /// <returns>
        /// A list of all teacher objects.
        /// </returns>
        [HttpGet]
        [Route("ListTeachers")]
        public List<Teacher> ListTeachers(string SearchKey = null)
        {
            // Create an empty list of teachers
            List<Teacher> teachers = new List<Teacher>();

            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                //command.CommandText = "SELECT * FROM teachers";

                string query = "SELECT * FROM teachers";

                //searh criteria
                if (SearchKey != null)
                {
                    query += " where lower(teacherfname) like lower(@key) or lower(teacherlname) like lower(@key) or lower(concat(teacherfname,' ',teacherlname)) like lower(@key) ";
                    command.Parameters.AddWithValue("@key", $"%{SearchKey}%");
                }
                Debug.WriteLine($"SearchKey: {SearchKey}");

                command.CommandText = query;
                command.Prepare();
                Debug.WriteLine(query);


                using (MySqlDataReader resultSet = command.ExecuteReader())
                {
                    // Loop through each row in the result set and populate the list
                    while (resultSet.Read())
                    {
                        teachers.Add(new Teacher
                        {
                            Id = Convert.ToInt32(resultSet["teacherid"]),
                            Name = $"{resultSet["teacherfname"]} {resultSet["teacherlname"]}",
                            EmployeeNumber = resultSet["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(resultSet["hiredate"])
                        });
                    }
                }
            }

            // Return the final list of teachers
            return teachers;
        }
        /// <summary>
        /// Adds an teacher to the database
        /// </summary>
        /// <param name="TeacherData">Teacher Object</param>
        /// <returns>
        /// The inserted Teacher Id from the database if successful. 0 if Unsuccessful
        /// </returns>
        [HttpPost(template: "AddTeacher")]
        public int AddTeacher([FromBody] Teacher TeacherData)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate) values (@teacherfname, @teacherlname, @employeenumber, @hiredate)";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.Teacherfname);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);

            }
            // if failure
            return 0;
        }


        /// <summary>
        /// Deletes an Teacher from the database
        /// </summary>
        /// <param name="TeacherId">Primary key of the teacher to delete</param>
        /// <example>
        /// DELETE: api/TeacherData/DeleteTeacher -> 1
        /// </example>
        /// <returns>
        /// Number of rows affected by delete operation.
        /// </returns>
        [HttpDelete(template: "DeleteTeacher/{TeacherId}")]
        public int DeleteTeacher(int TeacherId)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "delete from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", TeacherId);
                return Command.ExecuteNonQuery();

            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Updates an Teacher in the database. Data is Teacher object, request query contains ID
        /// </summary>
        /// <param name="TeacherData">Teacher Object</param>
        /// <param name="TeacherId">The Teacher ID primary key</param>
        /// <returns>
        /// The updated Teacher object
        /// </returns>
        [HttpPut(template: "UpdateTeacher/{TeacherId}")]
        public Teacher UpdateTeacher(int TeacherId, [FromBody] Teacher TeacherData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();
                // Validate if the Teacher exists
                Command.CommandText = "SELECT COUNT(*) FROM teachers WHERE teacherid=@id";
                Command.Parameters.AddWithValue("@id", TeacherId);
                int count = Convert.ToInt32(Command.ExecuteScalar());
                if (count == 0)
                {
                    throw new Exception("Teacher does not exist.");
                }

                // Validate Hire Date
                if (TeacherData.HireDate > DateTime.Now)
                {
                    throw new Exception("Hire date cannot be in the future.");
                }


                Command = Connection.CreateCommand();
                // parameterize query                                 
                Command.CommandText = "update teachers set teacherfname=@teacherfname, teacherlname=@teacherlname, employeenumber=@employeenumber, hiredate=@hiredate where teacherid=@id";
                Command.Parameters.AddWithValue("@teacherfname", TeacherData.Teacherfname);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);



                Command.Parameters.AddWithValue("@id", TeacherId);
                


                Command.ExecuteNonQuery();



            }

            return FindTeacher(TeacherId);
        }
    }
}
