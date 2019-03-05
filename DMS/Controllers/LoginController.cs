using DMS.Models;
using DMS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace DMS.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ProfileVM loginVM)
        {
            using (DMSDBContext dc = new DMSDBContext())
            {
                string message = "";
                var validate = dc.Users.FirstOrDefault(x => x.Email == loginVM.Email && x.Password == loginVM.Password);
                if (validate != null)
                {
                    var data = dc.Users.FirstOrDefault(a => a.Email == loginVM.Email);
                    FormsAuthentication.SetAuthCookie(data.Email, false);
                    Session["id"] = data.UserId;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    message = "Invalid credential provided";
                }
                ViewBag.Message = message;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            
            return RedirectToAction("Login", "Login");
        }
    }
}