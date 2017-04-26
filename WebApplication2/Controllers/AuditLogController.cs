using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Context;
using WebApplication2.Security;

namespace WebApplication2.Controllers
{
    public class AuditLogController : Controller
    {
        SelectList getAccountIDList(int? selectedID = null)
        {
            var items = AccountDbContext.getInstance().findAccounts();
            items.Insert(0, new Models.Account { AccountID = 0, Username = "" });
            return new SelectList(items, "AccountID", "Username", selectedID);
        }

        SelectList getLogActionList(string selectedID = null)
        {
            var items = new List<string>();
            items.Add("CREATE");
            items.Add("CREATE_NEW_VERSION");
            items.Add("EDIT");
            items.Add("EDIT_PROPERTIES");
            items.Add("SUBMIT_FOR_APPROVAL");
            items.Add("APPROVE");
            items.Add("UNAPPROVE");
            items.Add("PUBLISH");
            items.Add("UNPUBLISH");
            items.Insert(0, "");
            return new SelectList(items, selectedID);
        }

        // GET: AuditLog
        [CustomAuthorize(Roles = "superadmin")]
        public ActionResult Index(
            string accountID = "",
            string logAction = "", 
            string startDate = "", 
            string endDate = "",
            string category = "",
            string article = "")
        {
            var query = new AuditLogDbContext.Query();
            query.accountID = accountID;
            query.logAction = logAction;
            query.startDate = startDate;
            query.endDate = endDate;
            query.category = category;
            query.article = article;

            var list = AuditLogDbContext.getInstance().findAll(query);
            if (!accountID.Equals(""))
            {
                var id = Convert.ToInt32(accountID);
                ViewBag.accountID = getAccountIDList(id);
            }
            else
            {
                ViewBag.accountID = getAccountIDList();
            }

            ViewBag.logAction = getLogActionList(logAction);
            return View(list);
        }
    }
}