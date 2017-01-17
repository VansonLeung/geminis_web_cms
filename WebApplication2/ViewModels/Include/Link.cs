﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModels.Include
{
    public class Link
    {
        public Link(string locale, string category, string id)
        {
            this.locale = locale;
            this.category = category;
            this.id = id;
        }

        public bool is_external { get; set; }
        public bool is_absolute { get; set; }
        public string locale { get; set; }
        public string category { get; set; }
        public string id { get; set; }

        public string GetFullURL()
        {
            string url = "/";
            if (locale != null)
            {
                url += locale + "/";
            }
            if (category != null)
            {
                url += category + "/";
            }
            if (id != null)
            {
                url += id;
            }
            return url;
        }
    }
}