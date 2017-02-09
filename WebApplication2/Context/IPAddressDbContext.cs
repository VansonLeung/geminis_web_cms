using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Context
{
    public class IPAddressDbContext
    {
        // singleton

        private static IPAddressDbContext context;

        public static IPAddressDbContext getInstance()
        {
            if (context == null)
            {
                return new IPAddressDbContext();
            }
            return context;
        }


        // initializations 

        public BaseDbContext db = BaseDbContext.getInstance();

        public virtual DbSet<IPAddress> getItemDb()
        {
            return db.ipaddressDb;
        }
    }
}