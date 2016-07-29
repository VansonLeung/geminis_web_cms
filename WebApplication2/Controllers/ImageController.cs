using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

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
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/uploads"), FileName);
                upload.SaveAs(path);
            }
        }


        public ActionResult UploadPartial()
        {
            var appData = Server.MapPath("~/Images/uploads");
            var images = Directory.GetFiles(appData).Select(x => new ImageView
            {
                Url = Url.Content("/images/uploads/" + Path.GetFileName(x))
            });
            return View(images);
        }
    }

}