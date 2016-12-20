using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend.ViewModels
{
    public class Content
    {
        public string name { get; set; }
        public string slug { get; set; }
        public Link link { get; set; }
        public Category category { get; set; }
        public Listitem listitem { get; set; }
        public string desc { get; set; }
        public string type { get; set; }
        public string author { get; set; }
        public DateTime? datetime { get; set; }
        public string datetime_representation { get; set; }
    }
}