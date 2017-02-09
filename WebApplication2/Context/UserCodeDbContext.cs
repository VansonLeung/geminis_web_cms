using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Context
{
    public class UserCodeDbContext
    {
        // singleton

        private static UserCodeDbContext context;

        public static UserCodeDbContext getInstance()
        {
            if (context == null)
            {
                return new UserCodeDbContext();
            }
            return context;
        }


        // initializations 

        public BaseDbContext db = BaseDbContext.getInstance();

        public virtual DbSet<UserCode> getItemDb()
        {
            return db.userCodeDb;
        }
    }
}