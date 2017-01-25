using Frontend.Models;
using Frontend.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Frontend.Context
{
    public class IPAddressDbContext : FrontendBaseDbContext
    {
        // singleton

        private static IPAddressDbContext context;

        public static IPAddressDbContext getInstance()
        {
            if (context == null)
            {
                context = new IPAddressDbContext();
            }
            return context;
        }


        // initializations 

        private FrontendBaseDbContext db = FrontendBaseDbContext.getInstance();

        protected virtual DbSet<IPAddress> getItemDb()
        {
            return db.ipaddressDb;
        }
    }
}