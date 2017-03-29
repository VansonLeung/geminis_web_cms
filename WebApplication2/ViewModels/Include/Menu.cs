using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModels.Include
{
    public class Menu
    {
        public string name { get; set; }
        public Link link { get; set; }
        public ViewCategory category { get; set; }
        public bool is_active { get; set; }
        public bool is_highlighted { get; set; }
        public List<Menu> submenu { get; set; }
        public string desc { get; set; }
    }
}