using DMS.Models;
using DMS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        // GET: Category
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Index(string str)
        {
            using (DMSDBContext dc = new DMSDBContext())
            {
                List<ProfileVM> profileVM = new List<ProfileVM>();
                profileVM = dc.Categories.Select(x => new ProfileVM
                {
                    CategoryName = x.CategoryName,
                    CategoryId = x.CategoryId,
                    Email = x.User.Email
                }).ToList();

                if (!string.IsNullOrEmpty(str))
                {
                    var searcheditems = from x in profileVM.Where(x => x.CategoryName.Contains(str) || x.Email.Contains(str))
                                        select new ProfileVM
                                        {
                                            CategoryName = x.CategoryName,
                                            CategoryId = x.CategoryId,
                                            Email = x.Email
                                        };
                    return View(searcheditems);
                }
                return View(profileVM);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
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
                    var validate = dc.Categories.Where(a => a.CategoryName == profileVM.CategoryName).FirstOrDefault();
                    if (validate == null)
                    {
                        int id = Convert.ToInt32(Session["id"]);
                        Category category = new Category();
                        category.CategoryName = profileVM.CategoryName;
                        category.UserId = id;
                        dc.Categories.Add(category);
                        dc.SaveChanges();
                        message = "Category Created Successfully";
                        TempData["Message"] = message;
                        if (User.IsInRole("Admin"))
                        {
                            return RedirectToAction("Index", "Category");
                        }
                            return RedirectToAction("Create", "Category");
                    }
                    else
                    {
                        message = "This Category is already created. Please use another!!";
                        TempData["Message"] = message;
                        return RedirectToAction("Create", "Category");
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (DMSDBContext dc = new DMSDBContext())
            {
                var data = dc.Categories.Where(x => x.CategoryId == id).FirstOrDefault();
                ProfileVM profileVM = new ProfileVM()
                {
                    CategoryId = data.CategoryId,
                    CategoryName = data.CategoryName,
                };
                return View(profileVM);
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(ProfileVM profileVM)
        {
            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    string message = "";
                    var validate = dc.Categories.Where(a => a.CategoryName == profileVM.CategoryName).FirstOrDefault();
                    if (validate == null)
                    {
                        var data = dc.Categories.Where(x => x.CategoryId == profileVM.CategoryId).FirstOrDefault();
                        data.CategoryName = profileVM.CategoryName;
                        data.CategoryId = profileVM.CategoryId;
                        dc.Entry(data).State = EntityState.Modified;
                        dc.SaveChanges();
                        message = "Category Edited Successfully";
                        TempData["Message"] = message;
                        return RedirectToAction("Index", "Category");
                    }
                    else
                    {
                        message = "This Category is already created. Please use another!!";
                        TempData["Message"] = message;
                        return RedirectToAction("Create", "Category");
                    }
                }
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int id, ProfileVM profileVM)
        {
            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    var data = dc.Categories.Where(x => x.CategoryId == id).FirstOrDefault();
                    dc.Categories.Remove(data);
                    dc.SaveChanges();
                    string message = "Category Deleted Successfully";
                    TempData["Message"] = message;
                    return RedirectToAction("Index", "Category");
                }
            }
            catch
            {
                string message = "You are trying to delete category which is currently used by users";
                TempData["Message"] = message;
                return RedirectToAction("Index", "Category"); ;
            }
        }
    }
}