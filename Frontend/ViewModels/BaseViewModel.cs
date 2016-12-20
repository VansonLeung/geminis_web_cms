using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend.ViewModels
{
    public class BaseViewModel
    {
        public Category category { get; set; }
        public Content content { get; set; }
        public Current current { get; set; }
        public Lang lang { get; set; }
        public List list { get; set; }
        public Menu menu { get; set; }
        public Sideitem sideitem { get; set; }

        public static BaseViewModel make(string locale, string category, string id)
        {
            BaseViewModel vm = new BaseViewModel();
            vm.lang = new Lang();
            vm.lang.lang = locale;

            vm.category = new Category();
            vm.category.name = category;

            vm.content = new Content();
            vm.content.link = new Link();
            vm.content.link.url = id;
            return vm;
        }
    }
}