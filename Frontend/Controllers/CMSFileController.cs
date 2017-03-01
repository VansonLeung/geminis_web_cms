using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class CMSFileController : Controller
    {
        // GET: CMSFile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Redirect(string type, string path)
        {
            var constant = WebApplication2.Context.ConstantDbContext.getInstance().findActiveByKeyNoTracking("CMS_BASE_URL");
            
            if (constant == null)
            {
                return HttpNotFound();
            }
            
            if (type == null)
            {
                return HttpNotFound();
            }

            if (path == null)
            {
                return HttpNotFound();
            }

            return Redirect(constant.Value + "/ckfinder/userfiles/" + type + "/" + path);
        }
    }
}