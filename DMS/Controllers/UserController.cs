using DMS.Models;
using DMS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace DMS.Controllers
{
    [Authorize (Roles ="Admin")]
    public class UserController : Controller
    {
        // GET: User
        [HttpGet]
        public ActionResult Index(string str)
        {
            using (DMSDBContext dc = new DMSDBContext())
            {

                List<ProfileVM> profileVM = new List<ProfileVM>();
                profileVM = dc.Users.Select(x => new ProfileVM
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    DeptName = x.Department.DeptName,
                    UserId = x.UserId
                }).ToList();

                if (!string.IsNullOrEmpty(str))
                {
                    var searcheditems = from x in profileVM.Where(x => x.FirstName.Contains(str) || x.LastName.Contains(str) || x.Email.Contains(str) || x.DeptName.Contains(str))
                                        select new ProfileVM
                                        {
                                            FirstName = x.FirstName,
                                            LastName = x.LastName,
                                            Email = x.Email,
                                            DeptName = x.DeptName,
                                            UserId = x.UserId
                                        };
                    return View(searcheditems);
                }
                return View(profileVM);
            }

        }

        [HttpGet]
        public ActionResult Create()
        {
            DMSDBContext dc = new DMSDBContext();
            TempData["Departments"] = new SelectList(dc.Departments, "DeptId", "DeptName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProfileVM profileVM)
        {

            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    string message = "";
                    var validate = dc.Users.Where(a => a.Email == profileVM.Email).FirstOrDefault();
                    if (validate == null)
                    {
                        User user = new User();
                        user.FirstName = profileVM.FirstName;
                        user.LastName = profileVM.LastName;
                        user.Email = profileVM.Email;
                        user.Password = profileVM.NewPassword;
                        user.DeptId = profileVM.DeptId;
                        dc.Users.Add(user);
                        dc.SaveChanges();

                        SendEmail(user.Email, user.Password);
                        message = "User Created Successfully";
                        TempData["Message"] = message;
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        message = "This Email address is already Used. Please use another!!";
                        TempData["Message"] = message;
                        return RedirectToAction("Create", "User");
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            DMSDBContext dc = new DMSDBContext();
            var profile = dc.Users.Where(x => x.UserId == id).FirstOrDefault();
            ProfileVM profileVM = new ProfileVM()
            {
                UserId = profile.UserId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email,
                DeptId = profile.DeptId,
                DeptName = profile.Department.DeptName
            };
            TempData["Departments"] = new SelectList(dc.Departments, "DeptId", "DeptName");
            return View(profileVM);

        }

        [HttpPost]
        public ActionResult Edit(ProfileVM profileVM)
        {
            string message = "";
            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    var user = dc.Users.Where(x => x.UserId == profileVM.UserId).FirstOrDefault();
                    user.FirstName = profileVM.FirstName;
                    user.LastName = profileVM.LastName;
                    user.DeptId = profileVM.DeptId;
                    dc.Entry(user).State = EntityState.Modified;
                    dc.SaveChanges();
                    message = "User Edited Successfully";
                    TempData["Message"] = message;
                    return RedirectToAction("Index", "User");
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, ProfileVM profileVM)
        {
            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    var data = dc.Users.Where(x => x.UserId == id).FirstOrDefault();
                    dc.Users.Remove(data);
                    dc.SaveChanges();
                    string message = "User Deleted Successfully";
                    TempData["Message"] = message;
                    return RedirectToAction("Index", "User");
                }
            }
            catch
            {
                string message = "You are trying to delete User who have file!!";
                TempData["Message"] = message;
                return RedirectToAction("Index", "User");
            }
        }

        [NonAction]
        public void SendEmail(string emailID, string password)
        {
            var fromEmail = new MailAddress("documentmanagement07@gmail.com", "DMS");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "document147@";
            string subject = "Your account is successfully created!";

            string body = "Congratulation!! Your account is successfully created. You can access your account with given credentials. Thank You" +
                " Email : " + emailID + "</br> Password : " + password + ".";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}