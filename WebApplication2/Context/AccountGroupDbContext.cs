using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Models.Infrastructure;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class AccountGroupDbContext
    {
        // singleton

        private static AccountGroupDbContext context;

        public static AccountGroupDbContext getInstance()
        {
            if (context == null)
            {
                context = new AccountGroupDbContext();
            }
            return context;
        }


        // initializations 

        private BaseDbContext db = BaseDbContext.getInstance();

        protected virtual DbSet<AccountGroup> getItemDb()
        {
            return db.accountGroupDb;
        }



        // methods

        public List<AccountGroup> findGroups()
        {
            return getItemDb()
                .ToList();
        }
        public AccountGroup findGroupByID(int ID)
        {
            return getItemDb()
                .Where(acc => acc.AccountGroupID == ID)
                .FirstOrDefault();
        }

        public bool isDefaultGroupExists()
        {
            return getItemDb().Where(acc => acc.Name == "Default Group").Count() > 0;
        }

        public AccountGroup getDefaultGroup()
        {
            return getItemDb().Where(acc => acc.Name == "Default Group").FirstOrDefault();
        }


        public AccountGroup create(AccountGroup item)
        {
            getItemDb().Add(item);
            db.SaveChanges();
            return item;
        }

        public string edit(AccountGroup item)
        {
            var local = getItemDb()
                            .Local
                            .FirstOrDefault(f => f.AccountGroupID == item.AccountGroupID);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }

            db.Entry(item).State = EntityState.Modified;

            item.AccessibleArticleGroups = String.Join(",", item.getAccessibleArticleGroupList().ToArray());
            item.AccessibleContentPages = String.Join(",", item.getAccessibleContentPageList().ToArray());

            db.SaveChanges();
            return null;
        }

        public string delete(AccountGroup item)
        {
            getItemDb().Remove(item);
            db.SaveChanges();
            return null;
        }
    }
}