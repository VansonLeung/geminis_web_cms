using Frontend.Models;
using Frontend.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Frontend.Context
{
    public class UserDbContext : FrontendBaseDbContext
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

        private FrontendBaseDbContext db = FrontendBaseDbContext.getInstance();

        protected virtual DbSet<User> getItemDb()
        {
            return db.userDb;
        }
    }
}