using DMS.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DMS.Models.ViewModel;

namespace DMS.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        // GET: Profile
        [HttpGet]
        public ActionResult Index()
        {
            string email = User.Identity.Name;
            using (DMSDBContext dc = new DMSDBContext())
            {
                var profile = dc.Users.FirstOrDefault(c => c.Email == email);
                ProfileVM profileVM = new ProfileVM()
                {
                    UserId = profile.UserId,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    DeptName = profile.Department.DeptName
                };
                return View(profileVM);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            string email = User.Identity.Name;
            using (DMSDBContext dc = new DMSDBContext())
            {
                var profile = dc.Users.FirstOrDefault(c => c.Email == email);
                ProfileVM profileVM = new ProfileVM()
                {
                    UserId = profile.UserId,
                };
                return View(profileVM);
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(ProfileVM profileVM)
        {

            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    string message = "";

                    var validate = dc.Users.Where(a => a.Password == profileVM.Password).FirstOrDefault();
                    if (validate != null)
                    {
                        validate.Password = profileVM.NewPassword;
                        dc.Entry(validate).State = EntityState.Modified;
                        dc.SaveChanges();

                        message = "Password Changed Successfully";
                        TempData["Message"] = message;
                        return RedirectToAction("Index", "Profile");
                    }
                    else
                    {
                        message = "Input Valid Current Password";
                    }
                    ViewBag.Message = message;
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
    }
}