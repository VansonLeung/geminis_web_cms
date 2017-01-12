using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModels.Include
{
    public class Sideitem
    {
        // banner / widget / related
        public string type { get; set; }
        public string path { get; set; }
        public int id { get; set; }
        public int category_id { get; set; }
        public Link link { get; set; }
    }
}