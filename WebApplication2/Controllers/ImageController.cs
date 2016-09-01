using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Properties;

namespace WebApplication2.Controllers
{
    public class ImageController : Controller
    {
        public void UploadNow(HttpPostedFileWrapper upload)
        {
            if (upload != null)
            {
                string ImageName = upload.FileName;
                string[] FileNames = ImageName.Split('\\');
                string FileName = FileNames[FileNames.Length - 1];
                string path = System.IO.Path.Combine(Server.MapPath("~" + Settings.Default.MS_IMAGE_UPLOAD_SRC), FileName);
                upload.SaveAs(path);
            }
        }


        public ActionResult UploadForm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadForm(HttpPostedFileWrapper upload)
        {
            return View();
        }
    }

}