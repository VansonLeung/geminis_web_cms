using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Properties;

namespace WebApplication2.Controllers
{
    public class FileUploadController : Controller
    {
        public void UploadNow(HttpPostedFileWrapper upload)
        {
            if (upload != null)
            {
                string Filename = upload.FileName;
                string[] FileNames = Filename.Split('\\');
                string FileName = FileNames[FileNames.Length - 1];
                string path = System.IO.Path.Combine(Server.MapPath("~" + Settings.Default.MS_FILE_UPLOAD_SRC), FileName);
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