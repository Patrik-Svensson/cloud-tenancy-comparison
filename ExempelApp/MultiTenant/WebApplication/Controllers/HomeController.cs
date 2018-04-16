using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.DAL;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private SchoolContext db;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            if (!initTenantContext())
                return HttpNotFound();
            // Commenting out LINQ to show how to do the same thing in SQL.
            //IQueryable<EnrollmentDateGroup> = from student in db.Students
            //           group student by student.EnrollmentDate into dateGroup
            //           select new EnrollmentDateGroup()
            //           {
            //               EnrollmentDate = dateGroup.Key,
            //               StudentCount = dateGroup.Count()
            //           };

            // SQL version of the above LINQ code.
            string query = "SELECT EnrollmentDate, COUNT(*) AS StudentCount "
                + "FROM Person "
                + "WHERE Discriminator = 'Student' "
                + "GROUP BY EnrollmentDate";
            IEnumerable<EnrollmentDateGroup> data = db.Database.SqlQuery<EnrollmentDateGroup>(query);

            return View(data.ToList());
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if(db != null)
                db.Dispose();
            base.Dispose(disposing);
        }
        private bool initTenantContext()
        {
            var query = HttpContext.Request.QueryString.Get("TenantId");
            if (query != null)
            {
                db = new SchoolContext(Tenant.getTenant(HttpContext.Request.QueryString.Get("TenantId")).connectionString);
                return true;
            }

            return false;
        }
    }
}