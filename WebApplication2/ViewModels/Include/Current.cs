using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using static WebApplication2.Controllers.BaseController;

namespace WebApplication2.ViewModels.Include
{
    public class Current
    {
        public Current(BaseControllerSession session, Account me, ViewCategory page)
        {
            this.session = session;
            this.me = me;
            this.page = page;
        }

        public BaseControllerSession session { get; set; }
        public Account me { get; set; }
        public ViewCategory page { get; set; }
    }
}