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
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        // GET: Department
        [HttpGet]
        public ActionResult Index(string str)
        {
            using (DMSDBContext dc = new DMSDBContext())
            {
                List<ProfileVM> profileVM = new List<ProfileVM>();
                profileVM = dc.Departments.Select(x => new ProfileVM
                {
                    DeptName = x.DeptName,
                    DeptId = x.DeptId
                }).ToList();

                if (!string.IsNullOrEmpty(str))
                {
                    var searcheditems = from x in profileVM.Where(x => x.DeptName.Contains(str))
                                        select new ProfileVM
                                        {
                                            DeptName = x.DeptName,
                                            DeptId = x.DeptId
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
                    var validate = dc.Departments.Where(a => a.DeptName == profileVM.DeptName).FirstOrDefault();
                    if (validate == null)
                    {
                        Department department = new Department();
                        department.DeptName = profileVM.DeptName;
                        dc.Departments.Add(department);
                        dc.SaveChanges();
                        message = "Department Created Successfully";
                        TempData["Message"] = message;
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        message = "This Department is already created. Please use another!!";
                        TempData["Message"] = message;
                        return RedirectToAction("Create", "Department");
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
            using (DMSDBContext dc = new DMSDBContext())
            {
                var profile = dc.Departments.Where(x => x.DeptId == id).FirstOrDefault();
                ProfileVM profileVM = new ProfileVM()
                {
                    DeptId = profile.DeptId,
                    DeptName = profile.DeptName,
                };
                return View(profileVM);
            }

        }

        [HttpPost]
        public ActionResult Edit(ProfileVM profileVM)
        {
            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    string message = "";
                    var validate = dc.Departments.Where(a => a.DeptName == profileVM.DeptName).FirstOrDefault();
                    if (validate == null)
                    {
                        var data = dc.Departments.Where(x => x.DeptId == profileVM.DeptId).FirstOrDefault();
                        data.DeptName = profileVM.DeptName;
                        data.DeptId = profileVM.DeptId;
                        dc.Entry(data).State = EntityState.Modified;
                        dc.SaveChanges();
                        message = "Department Edited Successfully";
                        TempData["Message"] = message;
                        return RedirectToAction("Index", "Department");
                    }
                    else
                    {
                        message = "This Department is already created. Please use another!!";
                        TempData["Message"] = message;
                        return RedirectToAction("Create", "Department");
                    }
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
                    var data = dc.Departments.Where(x => x.DeptId == id).FirstOrDefault();
                    dc.Departments.Remove(data);
                    dc.SaveChanges();
                    string message = "Department Deleted Successfully";
                    TempData["Message"] = message;
                    return RedirectToAction("Index", "Department");
                }
            }
            catch
            {
                string message = "You are trying to delete department which is currently assigned to users";
                TempData["Message"] = message;
                return RedirectToAction("Index", "Department");
            }
        }
    }
}