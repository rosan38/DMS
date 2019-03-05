using System;
using System.Linq;
using System.Web.Mvc;
using DMS.Models;
using DMS.Models.ViewModel;

namespace DMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (DMSDBContext dc = new DMSDBContext())
            {
                var user = dc.Users.Count();
                var department = dc.Departments.Count();
                var category = dc.Categories.Count();
                if (User.IsInRole("Admin"))
                {
                    var document = dc.Documents.Count();
                    ViewBag.document = document;
                }
                else
                {
                    int id = Convert.ToInt32(Session["id"]);
                    var document = dc.Documents.Where(x => x.UserId == id).Count();
                    ViewBag.document = document;
                }

                ViewBag.user = user;
                ViewBag.department = department;
                ViewBag.category = category;
                return View();
            }
        }
    }
}
