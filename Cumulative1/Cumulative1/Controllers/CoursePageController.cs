using Cumulative1.Model;
using Microsoft.AspNetCore.Mvc;

namespace Cumulative1.Controllers
{
    public class CoursePageController : Controller
    {
        
        private readonly CourseAPIController _api;

        // Constructor for CoursePageController
        public CoursePageController(CourseAPIController api)
        {
            _api = api;
        }

        /// <summary>
        /// Displays a list of Courses on a dynamic page.
        /// </summary>
        /// <returns>A view containing all Courses.</returns>
        public IActionResult List(string SearchKey)
        {
            
            List<Course> Courses = _api.ListCourses(SearchKey);
            return View("~/Views/Course/List.cshtml", Courses);
        }


        /// <summary>
        /// Displays details of a specific Course by their ID.
        /// </summary>
        /// <param name="id">The ID of the Course.</param>
        /// <returns>A view containing the Course's details.</returns>
        public IActionResult Show(int id)
        {
            
            Course SelectedCourse = _api.FindCourse(id);
            return View("~/Views/Course/Show.cshtml", SelectedCourse);
        }

        // GET : CoursePage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View("~/Views/Course/New.cshtml");
        }

        // POST: CoursePage/Create
        [HttpPost]
        public IActionResult Create(Course NewCourse)
        {
            int CourseId = _api.AddCourse(NewCourse);
            return RedirectToAction("Show", new { id = CourseId });
        }

        // GET : CoursePage/DeleteConfirm/{id}
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Course SelectedCourse = _api.FindCourse(id);
            return View("~/Views/Course/DeleteConfirm.cshtml", SelectedCourse);
        }

        // POST: CoursePage/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int CourseId = _api.DeleteCourse(id);
           
            return RedirectToAction("List");
        }
        // GET : CoursePage/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Course SelectedCourse = _api.FindCourse(id);
            return View("~/Views/Course/Edit.cshtml", SelectedCourse);
        }

        // POST: CoursePage/Update/{id}
        [HttpPost]
        public IActionResult Update(int id, string Coursecode, int Teacherid, string Name, string Startdate, string Finishdate)
        {
            Course UpdatedCourse = new Course();
            UpdatedCourse.Coursecode = Coursecode;
            UpdatedCourse.Teacherid = Teacherid;
            UpdatedCourse.Name = Name;
            UpdatedCourse.Startdate = DateTime.Parse(Startdate);
            UpdatedCourse.Finishdate = DateTime.Parse(Finishdate);

            // not doing anything with the response
            _api.UpdateCourse(id, UpdatedCourse);
            // redirects to show course
            return RedirectToAction("Show", new { id = id });
        }
    }
}
