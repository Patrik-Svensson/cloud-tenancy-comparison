using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.DAL;
using WebApplication.Models;
using System.Data.Entity.Infrastructure;
using System.Runtime.Caching;

namespace WebApplication.Controllers
{
    public class CourseController : Controller
    {
        private readonly SchoolContext db;
        private readonly ICache _cache;
        private readonly ISettingsProvider _CatalogProvider;
        private readonly ITenantIdProvider _tenantId;

        public CourseController(SchoolContext ctx, ICache cache, ITenantIdProvider tenantId, ISettingsProvider _catalogProvider)
        {
            db = ctx;
            _cache = cache;
            this._tenantId = tenantId;
            _CatalogProvider = _catalogProvider;
        }

        // GET: Course
        public ActionResult Index(int? SelectedDepartment, int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            int departmentID;
            IEnumerable<Course> courses;

            var departments = db.Departments.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            departmentID = SelectedDepartment.GetValueOrDefault();
            courses = LoadCourses(SelectedDepartment, departmentID);

            return View(courses);
        }

        private IEnumerable<Course> LoadCourses(int? SelectedDepartment, int departmentID)
        {
            string cacheKey = $"CourseController.LoadCourses({SelectedDepartment},{departmentID})";

            List<Course> result = (List<Course>)_cache.Get(cacheKey);
            if (result == null)
            {
                result = LoadCoursesFromDatabase(SelectedDepartment, departmentID);
                _cache.Set(cacheKey, result, DateTimeOffset.Now.AddMinutes(1));
            }

            return result;
        }

        private List<Course> LoadCoursesFromDatabase(int? SelectedDepartment, int departmentID)
        {
            return db.Courses
                .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department)
                .ToList();
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id, int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        public ActionResult Create(int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID, TenantID")]Course course, int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { tenantId });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        public ActionResult Edit(int? id, int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //TODO FIXA KONTROLL
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var courseToUpdate = db.Courses.Find(id);
            if (TryUpdateModel(courseToUpdate, "",
               new string[] { "Title", "Credits", "DepartmentID" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index", new {tenantId});
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in db.Departments
                                   orderby d.Name
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }


        // GET: Course/Delete/5
        public ActionResult Delete(int? id, int? tenantID)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? tenantID)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateCourseCredits()
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCourseCredits(int? multiplier, int? tenantID)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            if (multiplier != null)
            {
                ViewBag.RowsAffected = db.Database.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
