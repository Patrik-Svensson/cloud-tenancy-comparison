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
        private readonly SchoolContext db;
        private readonly ITenantIdProvider _idProvider;
        private readonly ISettingsProvider _CatalogProvider;
        private QueryIdProvider provider = new QueryIdProvider();

        public HomeController(SchoolContext db, ITenantIdProvider idProvider, ISettingsProvider _catalogProvider)
        {
            this.db = db;
            this._idProvider = idProvider;
            _CatalogProvider = _catalogProvider;
        }

        public ActionResult Index(int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();

            return View();
        }

        public ActionResult About(int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
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

        public ActionResult Contact(int? tenantId)
        {
            ViewBag.KUNDNAMN = _CatalogProvider.GetDisplayName();
            ViewBag.Message = "Your contact page.";

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