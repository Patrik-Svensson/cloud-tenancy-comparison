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
        private readonly ITenantIdProvider tenantId;

        public CourseController(SchoolContext ctx, ICache cache, ITenantIdProvider tenantId)
        {
            db = ctx;
            _cache = cache;
            this.tenantId = tenantId;
        }

        // GET: Course
        public ActionResult Index(int? SelectedDepartment)
        {
            if (!initTenantContext())
                return HttpNotFound();

            int departmentID;
            IEnumerable<Course> courses;
            int testA = Convert.ToInt32(tenantId.TenantId());
            var departments = db.Departments.Where(x => x.TenantID == testA).OrderBy(q => q.Name).ToList();
            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            departmentID = SelectedDepartment.GetValueOrDefault();
            courses = LoadCourses(SelectedDepartment, departmentID, testA);

            return View(courses);
        }

        private IEnumerable<Course> LoadCourses(int? SelectedDepartment, int departmentID, int tenantID)
        {
            string cacheKey = $"CourseController.LoadCourses({SelectedDepartment},{departmentID})";

            List<Course> result = (List<Course>)_cache.Get(cacheKey);
            if (result == null)
            {
                result = LoadCoursesFromDatabase(SelectedDepartment, departmentID, tenantID);
                _cache.Set(cacheKey, result, DateTimeOffset.Now.AddMinutes(1));
            }

            return result;
        }

        private List<Course> LoadCoursesFromDatabase(int? SelectedDepartment, int departmentID, int tenantID)
        {
            return db.Courses
                .Where(c => (!SelectedDepartment.HasValue || c.DepartmentID == departmentID) && c.TenantID == tenantID)
                .OrderBy(d => d.CourseID)
                .Include(d => d.Department)
                .ToList();
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (!initTenantContext())
                return HttpNotFound();

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

        public ActionResult Create()
        {
            if (!initTenantContext())
                return HttpNotFound();

            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")]Course course)
        {
            if (!initTenantContext())
                return HttpNotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Index");
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

        public ActionResult Edit(int? id)
        {
            if (!initTenantContext())
                return HttpNotFound();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //TODO FIXA KONTROLL
            //int testA = Convert.ToInt32(tenantId.TenantId());
            //Course course = db.Courses.Where(x => x.TenantID == testA).Find(id);
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
        public ActionResult EditPost(int? id)
        {
            if (!initTenantContext())
                return HttpNotFound();

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

                    return RedirectToAction("Index");
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
        public ActionResult Delete(int? id)
        {
            if (!initTenantContext())
                return HttpNotFound();

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
        public ActionResult DeleteConfirmed(int id)
        {
            if (!initTenantContext())
                return HttpNotFound();

            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCourseCredits(int? multiplier)
        {
            if (!initTenantContext())
                return HttpNotFound();

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

        private bool initTenantContext()
        {
            return true;
        }
    }
}
