using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace Frontend.ViewModels
{
    public class Current
    {
        public string clientID { get; set; }
        public string sessionID { get; set; }
        public Account me { get; set; }
        public Category page { get; set; }
    }
}