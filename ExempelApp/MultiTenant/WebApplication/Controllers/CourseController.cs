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
        // TODO: En context ska vara korfattad, sedan "dispose"
        private SchoolContext db;// = new SchoolContext()
        // GET: Course
        public ActionResult Index(int? SelectedDepartment)
        {
            var query = HttpContext.Request.QueryString.Get("Id");
            int departmentID;
            IEnumerable<Course> courses;

            if (query != null)
            {
                try
                {
                    db = Tenant.getTenant(query).db;
                }catch(NullReferenceException e)
                {
                    return HttpNotFound();
                }
                var departments = db.Departments.OrderBy(q => q.Name).ToList();
                ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
                departmentID = SelectedDepartment.GetValueOrDefault();

                courses = LoadCourses(SelectedDepartment, departmentID);
            }
            else
                return HttpNotFound();

            return View(courses);
        }

        // TODO: Find solution for tenant aware caching
        private IEnumerable<Course> LoadCourses(int? SelectedDepartment, int departmentID)
        {
            // Check for object in cache, if it is 
            // Here the default memory cache is used directly
            var cache = MemoryCache.Default;
            string cacheKey = $"CourseController.LoadCourses({SelectedDepartment},{departmentID})";

            List<Course> result = (List<Course>)cache.Get(cacheKey);
            if (result == null)
            {
                result = LoadCoursesFromDatabase(SelectedDepartment, departmentID);

                // Cache for 
                cache.Add(cacheKey, result, DateTimeOffset.Now.AddMinutes(1));
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
        public ActionResult Details(int? id)
        {
            var query = HttpContext.Request.QueryString.Get("Id");
            Course course = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
                course = db.Courses.Find(id);
            }
            else
                return HttpNotFound();
       
            return View(course);
        }


        public ActionResult Create()
        {
            var query = HttpContext.Request.QueryString.Get("Id");
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
            }
            else
                return HttpNotFound();

            PopulateDepartmentsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")]Course course)
        {
            var query = HttpContext.Request.QueryString.Get("Id");
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
            }
            else
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
            var query = HttpContext.Request.QueryString.Get("Id");
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
            }
            else
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
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            var query = HttpContext.Request.QueryString.Get("Id");
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
            }
            else
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
            var query = HttpContext.Request.QueryString.Get("Id");
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
            }
            else
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
            var query = HttpContext.Request.QueryString.Get("Id");
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
            }
            else
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
            var query = HttpContext.Request.QueryString.Get("Id");
            if (query != null)
            {
                db = Tenant.getTenant(query).db;
            }
            else
                return HttpNotFound();

            if (multiplier != null)
            {
                ViewBag.RowsAffected = db.Database.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
            }
            return View();
        }

        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
