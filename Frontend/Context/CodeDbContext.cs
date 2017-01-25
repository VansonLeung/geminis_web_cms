using Frontend.Models;
using Frontend.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Frontend.Context
{
    public class CodeDbContext : FrontendBaseDbContext
    {
        // singleton

        private static CodeDbContext context;

        public static CodeDbContext getInstance()
        {
            if (context == null)
            {
                context = new CodeDbContext();
            }
            return context;
        }


        // initializations 

        private FrontendBaseDbContext db = FrontendBaseDbContext.getInstance();

        protected virtual DbSet<Code> getItemDb()
        {
            return db.codeDb;
        }
    }
}