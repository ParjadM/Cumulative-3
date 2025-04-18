using Microsoft.AspNetCore.Mvc;
using Cumulative1.Model;

namespace Cumulative1.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly TeacherAPIController _api;
        public TeacherPageController(TeacherAPIController api)
        {
            _api = api; 
        }

        /// <summary>
        /// Displays a list of teachers on a dynamic page.
        /// </summary>
        /// <returns>A view containing all teachers.</returns>
        public IActionResult List(string SearchKey)
        {
            List<Teacher> Teachers =_api.ListTeachers(SearchKey);
            return View("~/Views/Teacher/List.cshtml", Teachers);
        }


        /// <summary>
        /// Displays details of a specific teacher by their ID.
        /// </summary>
        /// <param name="id">The ID of the teacher.</param>
        /// <returns>A view containing the teacher's details.</returns>
        public IActionResult Show(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View("~/Views/Teacher/Show.cshtml",SelectedTeacher);
        }


        // GET : TeacherPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View("~/Views/Teacher/New.cshtml");
        }

        // POST: TeacherPage/Create
        [HttpPost]
        public IActionResult Create(Teacher NewTeacher)
        {
            int TeacherId = _api.AddTeacher(NewTeacher);
            return RedirectToAction("Show", new { id = TeacherId });
        }

        // GET : TeacherPage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View("~/Views/Teacher/DeleteConfirm.cshtml",SelectedTeacher);
        }

        // POST: TeacherPage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int TeacherId = _api.DeleteTeacher(id);
            return RedirectToAction("List");
        }

        // GET : TeacherPage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Teacher SelectedTeacher = _api.FindTeacher(id);
            return View("~/Views/Teacher/Edit.cshtml", SelectedTeacher);
        }

        // POST: TeacherPage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string TeacherFName, string TeacherLName, string EmployeeNumber, string HireDate)
        {
            Teacher UpdatedTeacher = new Teacher();
            UpdatedTeacher.Teacherfname = TeacherFName;
            UpdatedTeacher.TeacherLName = TeacherLName;
            UpdatedTeacher.EmployeeNumber = EmployeeNumber;
            UpdatedTeacher.HireDate = DateTime.Parse(HireDate);

            // not doing anything with the response
            _api.UpdateTeacher(id, UpdatedTeacher);
            // redirects to show teacher
            return RedirectToAction("Show", new { id = id });
        }

    }
}
