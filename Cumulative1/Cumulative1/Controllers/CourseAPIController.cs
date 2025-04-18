using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative1.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cumulative1.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        // Dependency injection of database context
        public CourseAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns detailed information about a specific course by ID.
        /// </summary>
        /// <example>
        /// GET api/Course/FindCourse/1
        /// </example>
        /// <param name="id">The ID of the course to retrieve.</param>
        /// <returns>
        /// A course object containing detailed information.
        /// </returns>
        [HttpGet]
        [Route("FindCourse/{id}")]
        public Course FindCourse(int id)
        {
            // Initialize an empty Course object
            Course selectedCourse = null;

            using (MySqlConnection connection =_context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText ="SELECT * FROM Courses WHERE Courseid = @id";
                command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader resultSet= command.ExecuteReader())
                {
                    // If a matching course is found, populate the course object
                    if (resultSet.Read())
                    {
                        selectedCourse = new Course
                        {
                            Id = Convert.ToInt32(resultSet["Courseid"]),
                            Name = resultSet["coursename"].ToString(),
                            Teacherid = Convert.ToInt32(resultSet["teacherid"]),
                            Startdate = Convert.ToDateTime(resultSet["startdate"]),
                            Finishdate = Convert.ToDateTime(resultSet["finishdate"]),
                            Coursecode = resultSet["coursecode"].ToString()
                        };
                    }
                }
            }

            // Return the selected course or null if not found
            return selectedCourse;
        }

        /// <summary>
        /// Returns a list of all courses in the system.
        /// </summary>
        /// <example>
        /// GET api/Course/ListCourses 
        /// </example>
        /// <returns>
        /// A list of all course objects.
        /// </returns>
        [HttpGet]
        [Route("ListCourses")]
        public List<Course> ListCourses(string SearchKey = null)
        {
            // Create an empty list of courses
            List<Course>courses = new List<Course>();

            using (MySqlConnection connection= _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                //command.CommandText = "SELECT * FROM courses";

                string query = "SELECT * FROM courses";

                //searh criteria
                if (SearchKey != null)
                {
                    query += " where lower(coursename) like lower(@key)";
                    command.Parameters.AddWithValue("@key", $"%{SearchKey}%");
                }
                Debug.WriteLine($"SearchKey: {SearchKey}");

                command.CommandText = query;
                command.Prepare();
                Debug.WriteLine(query);

                using (MySqlDataReader resultSet = command.ExecuteReader())
                {
                    //loop through each row in the result set and populate the list
                    while (resultSet.Read())
                    {
                        courses.Add(new Course
                        {
                            Id = Convert.ToInt32(resultSet["Courseid"]),
                            Name = resultSet["coursename"].ToString(),
                            Teacherid = Convert.ToInt32(resultSet["teacherid"]),
                            Startdate = Convert.ToDateTime(resultSet["startdate"]),
                            Finishdate = Convert.ToDateTime(resultSet["finishdate"]),
                            Coursecode = resultSet["coursecode"].ToString()
                        });
                    }
                }
            }

            //Return the final list of courses
            return courses;
        }

        /// <summary>
        /// Adds an course to the database
        /// </summary>
        /// <param name="CourseData">Course Object</param>
        /// <returns>
        /// The inserted Course Id from the database if successful. 0 if Unsuccessful
        /// </returns>
        [HttpPost(template: "AddCourse")]
        public int AddCourse([FromBody] Course CourseData)
        {
           
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "insert into courses (Coursecode, Teacherid, coursename, Startdate,Finishdate) values (@Coursecode, @Teacherid, @coursename, @Startdate,@Finishdate)";
                Command.Parameters.AddWithValue("@Coursecode", CourseData.Coursecode);
                Command.Parameters.AddWithValue("@Teacherid", CourseData.Teacherid);
                Command.Parameters.AddWithValue("@coursename", CourseData.Name);
                Command.Parameters.AddWithValue("@Startdate", CourseData.Startdate);
                Command.Parameters.AddWithValue("@Finishdate", CourseData.Finishdate);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);

            }
            // if failure
            return 0;
        }


        /// <summary>
        /// Deletes an Course from the database
        /// </summary>
        /// <param name="CourseId">Primary key of the course to delete</param>
        /// <example>
        /// DELETE: api/CourseData/DeleteCourse -> 1
        /// </example>
        /// <returns>
        /// Number of rows affected by delete operation.
        /// </returns>
        [HttpDelete(template: "DeleteCourse/{CourseId}")]
        public int DeleteCourse(int CourseId)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "delete from courses where courseid=@id";
                Command.Parameters.AddWithValue("@id", CourseId);
                return Command.ExecuteNonQuery();

            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Updates an Course in the database. Data is Course object, request query contains ID
        /// </summary>
        /// <param name="CourseData">Course Object</param>
        /// <param name="CourseId">The Course ID primary key</param>
        /// <returns>
        /// The updated Course object
        /// </returns>
        [HttpPut(template: "UpdateCourse/{CourseId}")]
        public Course UpdateCourse(int CourseId, [FromBody] Course CourseData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // parameterize query                                
                Command.CommandText = "update courses set Coursecode=@Coursecode, Teacherid=@Teacherid, coursename=@coursename, Startdate=@Startdate, Finishdate=@Finishdate  where courseid=@id";
                Command.Parameters.AddWithValue("@Coursecode", CourseData.Coursecode);
                Command.Parameters.AddWithValue("@Teacherid", CourseData.Teacherid);
                Command.Parameters.AddWithValue("@coursename", CourseData.Name);
                Command.Parameters.AddWithValue("@Startdate", CourseData.Startdate);
                Command.Parameters.AddWithValue("@Finishdate", CourseData.Finishdate);

                Command.Parameters.AddWithValue("@id", CourseId);

                Command.ExecuteNonQuery();



            }

            return FindCourse(CourseId);
        }
    }
}
