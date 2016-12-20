using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend.ViewModels
{
    public class Menu
    {
        public string name { get; set; }
        public Link link { get; set; }
        public Category category { get; set; }
        public bool is_active { get; set; }
        public bool is_highlighted { get; set; }
    }
}