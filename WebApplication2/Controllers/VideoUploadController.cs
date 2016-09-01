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
                string fileExtension = fileName.Split('.').LastOrDefault();
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method

                if (!mimeType.Equals("video/mp4")
                    || !fileExtension.Equals("mp4"))
                {
                    var ffmpeg = new NReco.VideoConverter.FFMpegConverter();

                    var original_video_src = Server.MapPath("~" + Settings.Default.MS_VIDEO_UPLOAD_SRC) + "Orig_" + fileName;
                    var converted_video_src = Server.MapPath("~" + Settings.Default.MS_VIDEO_UPLOAD_SRC) + fileName;

                    file.SaveAs(original_video_src);
                    ffmpeg.ConvertMedia(original_video_src, converted_video_src, "mp4");

                    if (System.IO.File.Exists(original_video_src))
                    {
                        System.IO.File.Delete(original_video_src);
                    }
                }
                else
                {
                    var converted_video_src = Server.MapPath("~" + Settings.Default.MS_VIDEO_UPLOAD_SRC) + fileName;
                    file.SaveAs(converted_video_src);
                }
            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }
    }
}