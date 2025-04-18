using Cumulative1.Model;
using Microsoft.AspNetCore.Mvc;

namespace Cumulative1.Controllers
{
    public class StudentPageController : Controller
    {
        
        private readonly StudentAPIController _api;

        
        public StudentPageController(StudentAPIController api)
        {
            _api =api;
        }

        /// <summary>
        /// Displays a list of Students on a dynamic page.
        /// </summary>
        /// <returns>A view containing all Students.</returns>
        public IActionResult List(string SearchKey)
        {
            
            List<Student> Students = _api.ListStudents(SearchKey);

            
            return View("~/Views/Student/List.cshtml", Students);
        }


        /// <summary>
        /// Displays details of a specific Student by their ID.
        /// </summary>
        /// <param name="id">The ID of the Student.</param>
        /// <returns>A view containing the Student's details.</returns>
        public IActionResult Show(int id)
        {
            Student SelectedStudent =_api.FindStudent(id);
            return View("~/Views/Student/Show.cshtml", SelectedStudent);
        }


        // GET : StudentPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View("~/Views/Student/New.cshtml");
        }

        // POST: StudentPage/Create
        [HttpPost]
        public IActionResult Create(Student NewStudent)
        {
            int StudentId = _api.AddStudent(NewStudent);

            
            return RedirectToAction("Show", new { id = StudentId });
        }

        // GET : StudentPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Student SelectedStudent = _api.FindStudent(id);
            return View("~/Views/Student/DeleteConfirm.cshtml", SelectedStudent);
        }

        // POST: StudentPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int StudentId = _api.DeleteStudent(id);
            
            return RedirectToAction("List");
        }
        // GET : StudentPage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Student SelectedStudent = _api.FindStudent(id);
            return View("~/Views/Student/Edit.cshtml", SelectedStudent);
        }

        // POST: StudentPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string StudentFName, string StudentLName, string Studentnumber, string Enroldate)
        {
            Student UpdatedStudent = new Student();
            UpdatedStudent.Studentfname = StudentFName;
            UpdatedStudent.StudentLName = StudentLName;
            UpdatedStudent.Studentnumber = Studentnumber;
            UpdatedStudent.Enroldate = DateTime.Parse(Enroldate);

            // not doing anything with the response
            _api.UpdateStudent(id, UpdatedStudent);
            // redirects to show student
            return RedirectToAction("Show", new { id = id });
        }
    }
}
