using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Properties;

namespace WebApplication2.Controllers
{
    public class VideoUploadController : Controller
    {
        // GET: FileUpload
        public void UploadNow(HttpPostedFileWrapper upload)
        {
            if (upload != null)
            {
                string Filename = upload.FileName;
                string[] FileNames = Filename.Split('\\');
                string FileName = FileNames[FileNames.Length - 1];
                string path = System.IO.Path.Combine(Server.MapPath("~" + Settings.Default.MS_VIDEO_UPLOAD_SRC), FileName);
                upload.SaveAs(path);
            }
        }


        public ActionResult UploadPage()
        {
            return View();
        }

        

        [HttpPost]
        public JsonResult UploadAPI()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string filePath = file.FileName;
                string fileName = filePath.Split('\\').LastOrDefault();
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(Server.MapPath("~" + Settings.Default.MS_VIDEO_UPLOAD_SRC) + fileName); //File will be saved in application root
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }
    }
}