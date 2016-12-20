using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend.ViewModels
{
    public class Link
    {
        public string url { get; set; }
        public bool is_external { get; set; }
        public bool is_absolute { get; set; }
    }
}