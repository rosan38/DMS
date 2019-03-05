using DMS.Models;
using DMS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMS.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        // GET: Document
        [HttpGet]
        public ActionResult Index(string str)
        {
            using (DMSDBContext dc = new DMSDBContext())
            {
                List<ProfileVM> profileVM = new List<ProfileVM>();
                if (User.IsInRole("Admin"))
                {
                    profileVM = dc.Documents.Select(x => new ProfileVM
                    {
                        DocumentId = x.DocumentId,
                        DocumentName = x.DocumentName,
                        DocumentPath = x.DocumentPath,
                        DocumentDetails = x.DocumentDetails,
                        CategoryName = x.Category.CategoryName,
                        Email = x.User.Email
                    }).ToList();
                }
                else
                {
                    int id = Convert.ToInt32(Session["id"]);

                    
                    profileVM = dc.Documents.Where(x => x.UserId == id).Select(x => new ProfileVM
                    {
                        DocumentId = x.DocumentId,
                        DocumentName = x.DocumentName,
                        DocumentPath = x.DocumentPath,
                        DocumentDetails = x.DocumentDetails,
                        CategoryName = x.Category.CategoryName,
                        Email = x.User.Email
                    }).ToList();
                }
                if (!string.IsNullOrEmpty(str))
                {
                    var searcheditems = from x in profileVM.Where(x => x.DocumentName.Contains(str) || x.DocumentName.Contains(str) || x.CategoryName.Contains(str) || x.Email.Contains(str))
                                        select new ProfileVM
                                        {
                                            DocumentId = x.DocumentId,
                                            DocumentName = x.DocumentName,
                                            DocumentPath = x.DocumentPath,
                                            DocumentDetails = x.DocumentDetails,
                                            CategoryName = x.CategoryName,
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
            using (DMSDBContext dc = new DMSDBContext())
            {
                var data = dc.Categories.ToList();
                var result = new List<SelectListItem>();
                foreach (var item in data)
                {
                    result.Add(new SelectListItem
                    {
                        Value = item.CategoryId.ToString(),
                        Text = item.CategoryName,
                    });
                }
                TempData["category"] = new SelectList(result, "Value", "Text");
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file, ProfileVM profileVM)
        {
            try
            {
                using (DMSDBContext dc = new DMSDBContext())
                {
                    string message = "";
                    int id = Convert.ToInt32(Session["id"]);
                    var validate = (from p in dc.Documents
                                    where (p.DocumentName == file.FileName) && (p.CategoryId == profileVM.CategoryId)
                                    select p
                                     ).FirstOrDefault();
                    if (file.ContentLength > 0 && validate == null)
                    {
                        string FileName = Path.GetFileName(file.FileName);
                        var category = dc.Categories.Where(a => a.CategoryId == profileVM.CategoryId).FirstOrDefault();
                        string path = Path.Combine(Server.MapPath("//Documents/" + category.CategoryName));
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string paths = Path.Combine(Server.MapPath("//Documents/" + category.CategoryName), FileName);
                        Document document = new Document();
                        document.DocumentName = FileName;
                        document.DocumentPath = path.ToString();
                        document.DocumentDetails = profileVM.DocumentDetails;
                        document.CategoryId = profileVM.CategoryId;
                        document.UserId = id;
                        dc.Documents.Add(document);
                        dc.SaveChanges();
                        file.SaveAs(paths);
                        message = "Document Uploaded Successfully";
                        TempData["Message"] = message;
                        return RedirectToAction("Index", "Document");
                    }
                    else
                    {
                        message = "Document size is zero OR same file name already present!!";
                        TempData["Message"] = message;
                        return RedirectToAction("Create", "Document");
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
                    int uid = Convert.ToInt32(Session["id"]);
                    var data = dc.Documents.Where(x => x.DocumentId == id).FirstOrDefault();
                    string strPhysicalFolder = Server.MapPath("//Documents/" + data.Category.CategoryName + "/");
                    string strFileFullPath = strPhysicalFolder + data.DocumentName;
                    if (System.IO.File.Exists(strFileFullPath))
                    {
                        System.IO.File.Delete(strFileFullPath);
                        dc.Documents.Remove(data);
                        dc.SaveChanges();
                    }
                    string message = "Document Deleted Successfully";
                    TempData["Message"] = message;
                    return RedirectToAction("Index", "Document");
                }
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Download(int id)
        {
            using (DMSDBContext dc = new DMSDBContext())
            {
                var data = dc.Documents.Where(x => x.DocumentId == id).FirstOrDefault();
                string strPhysicalFolder = Server.MapPath("//Documents/" + data.Category.CategoryName + "/");
                string strFileFullPath = strPhysicalFolder + data.DocumentName;
                return File(strFileFullPath, System.Net.Mime.MediaTypeNames.Application.Octet, data.DocumentName);
            }
        }
    }
}