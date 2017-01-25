using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModels.Include
{
    public class ViewContent
    {
        public string name { get; set; }
        public string slug { get; set; }
        public Link link { get; set; }
        public ViewCategory category { get; set; }
        public Listitem listitem { get; set; }
        public List<Listitem> articleList { get; set; }
        public int articleListTotal { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public string author { get; set; }
        public DateTime? datetime { get; set; }
        public string datetime_representation { get; set; }
    }
}