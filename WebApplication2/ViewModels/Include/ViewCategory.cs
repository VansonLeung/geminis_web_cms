using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModels.Include
{ 
    public class ViewCategory
    {
        public ViewCategory()
        {

        }

        public ViewCategory(WebApplication2.Models.Infrastructure.Category cat, Lang lang)
        {
            if (cat != null && lang != null)
            {
                this.locale = lang.locale;
                this.categoryItemID = cat.ItemID;
                this.categoryParentItemID = cat.parentItemID.GetValueOrDefault(0);
                this.name = cat.getUrl();
                this.title = cat.GetName(lang.lang);
                this.is_active = cat.isEnabled;
                this.is_disabled = !cat.isEnabled;
                this.link = new Link(lang.locale, cat.getUrl(), null);
            }
        }
        
        public bool isNoContent { get; set; }
        public string locale { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public Link link { get; set; }
        public string type { get; set; }
        public bool is_active { get; set; }
        public bool is_disabled { get; set; }
        public int categoryItemID { get; set; }
        public int categoryParentItemID { get; set; }
        public ViewCategory categoryParent { get; set; }
        public List<Menu> submenu { get; set; }
    }
}