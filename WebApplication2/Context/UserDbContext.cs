using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Context
{
    public class UserDbContext
    {
        // singleton

        private static UserDbContext context;

        public static UserDbContext getInstance()
        {
            if (context == null)
            {
                context = new UserDbContext();
            }
            return context;
        }


        // initializations 

        private BaseDbContext db = BaseDbContext.getInstance();

        protected virtual DbSet<User> getItemDb()
        {
            return db.userDb;
        }
    }
}