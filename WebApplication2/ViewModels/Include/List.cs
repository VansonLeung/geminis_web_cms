using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModels.Include
{
    public class List
    {
        public int page_total { get; set; }
        public int page_cur { get; set; }
        public int items_total { get; set; }
        public List<Listitem> listitems { get; set; }
    }
}