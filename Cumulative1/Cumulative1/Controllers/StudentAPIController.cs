using Microsoft.AspNetCore.Mvc;
using Cumulative1.Model;
using MySql.Data.MySqlClient;
using System.Diagnostics;
namespace Cumulative1.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        // Dependency injection of database context
        public StudentAPIController(SchoolDbContext context)
        {
            _context =context;
        }

        /// <summary>
        /// Returns detailed information about a specific student by ID.
        /// </summary>
        /// <example>
        /// GET api/Student/FindStudent/1 
        /// </example>
        /// <param name="id">The ID of the student to retrieve.</param>
        /// <returns>
        /// A student object containing detailed information.
        /// </returns>
        [HttpGet]
        [Route("FindStudent/{id}")]
        public Student FindStudent(int id)
        {
            // Initialize an empty Student object
            Student selectedStudent = null;

            using (MySqlConnection connection =_context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText= "SELECT * FROM Students WHERE StudentId = @id";
                command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader resultSet = command.ExecuteReader())
                {
                    // If a matching student is found, populate the student object
                    if (resultSet.Read())
                    {
                        selectedStudent = new Student
                        {
                            Id = Convert.ToInt32(resultSet["StudentId"]),
                            Name = $"{resultSet["studentfname"]} {resultSet["studentlname"]}",
                            Studentnumber = resultSet["studentnumber"].ToString(),
                            Enroldate = Convert.ToDateTime(resultSet["enroldate"])
                        };
                    }
                }
            }

            // Return the selected student or null if not found
            return selectedStudent;
        }

        /// <summary>
        /// Returns a list of all students in the system.
        /// </summary>
        /// <example>
        /// GET api/Student/ListStudents
        /// </example>
        /// <returns>
        /// A list of all student objects.
        /// </returns>
        [HttpGet]
        [Route("ListStudents")]
        public List<Student> ListStudents(string SearchKey = null)
        {
            // Create an empty list of students
            List<Student> students = new List<Student>();

            
            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                //command.CommandText = "SELECT * FROM students";

                string query = "SELECT * FROM students";

                //searh criteria
                if (SearchKey != null)
                {
                    query += " where lower(studentfname) like lower(@key) or lower(studentlname) like lower(@key) or lower(concat(studentfname,' ',studentlname)) like lower(@key) ";
                    command.Parameters.AddWithValue("@key", $"%{SearchKey}%");
                }
                Debug.WriteLine($"SearchKey: {SearchKey}");

                command.CommandText = query;
                command.Prepare();
                Debug.WriteLine(query);

                using (MySqlDataReader resultSet= command.ExecuteReader())
                {
                    // Loop through each row in the result set and populate the list
                    while (resultSet.Read())
                    {
                        students.Add(new Student
                        {
                            Id = Convert.ToInt32(resultSet["StudentId"]),
                            Name = $"{resultSet["studentfname"]} {resultSet["studentlname"]}",
                            Studentnumber = resultSet["studentnumber"].ToString(),
                            Enroldate = Convert.ToDateTime(resultSet["enroldate"])
                        });
                    }
                }
            }

            // Return the final list of students
            return students;
        }
        /// <summary>
        /// Adds an student to the database
        /// </summary>
        /// <param name="StudentData">Student Object</param>
        /// <returns>
        /// The inserted Student Id from the database if successful. 0 if Unsuccessful
        /// </returns>
        [HttpPost(template: "AddStudent")]
        public int AddStudent([FromBody] Student StudentData)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // CURRENT_DATE() for the student join date in this context
                // Other contexts the join date may be an input criteria!
                Command.CommandText = "insert into students (studentfname, studentLName, studentnumber, enroldate) values (@Studentfname, @StudentLName, @Studentnumber, @Enroldate)";
                Command.Parameters.AddWithValue("@studentfname", StudentData.Studentfname);
                Command.Parameters.AddWithValue("@studentLName", StudentData.StudentLName);
                Command.Parameters.AddWithValue("@studentnumber", StudentData.Studentnumber);
                Command.Parameters.AddWithValue("@enroldate", StudentData.Enroldate);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);

            }
            // if failure
            return 0;
        }


        /// <summary>
        /// Deletes an Student from the database
        /// </summary>
        /// <param name="StudentId">Primary key of the student to delete</param>
        /// <example>
        /// DELETE: api/StudentData/DeleteStudent -> 1
        /// </example>
        /// <returns>
        /// Number of rows affected by delete operation.
        /// </returns>
        [HttpDelete(template: "DeleteStudent/{StudentId}")]
        public int DeleteStudent(int StudentId)
        {

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "delete from students where studentid=@id";
                Command.Parameters.AddWithValue("@id", StudentId);
                return Command.ExecuteNonQuery();

            }
            // if failure
            return 0;
        }
        /// <summary>
        /// Updates an Student in the database. Data is Student object, request query contains ID
        /// </summary>
        /// <param name="StudentData">Student Object</param>
        /// <param name="StudentId">The Student ID primary key</param>
        /// <returns>
        /// The updated Student object
        /// </returns>
        [HttpPut(template: "UpdateStudent/{StudentId}")]
        public Student UpdateStudent(int StudentId, [FromBody] Student StudentData)
        {
            // 'using' will close the connection after the code executes
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                //Establish a new command (query) for our database
                MySqlCommand Command = Connection.CreateCommand();

                // parameterize query                                
                Command.CommandText = "update students set studentfname=@studentfname, studentLName=@studentLName, studentnumber=@studentnumber, enroldate=@enroldate where studentid=@id";
                Command.Parameters.AddWithValue("@studentfname", StudentData.Studentfname);
                Command.Parameters.AddWithValue("@studentLName", StudentData.StudentLName);
                Command.Parameters.AddWithValue("@studentnumber", StudentData.Studentnumber);
                Command.Parameters.AddWithValue("@enroldate", StudentData.Enroldate);

                Command.Parameters.AddWithValue("@id", StudentId);

                Command.ExecuteNonQuery();



            }

            return FindStudent(StudentId);
        }
    }
}
