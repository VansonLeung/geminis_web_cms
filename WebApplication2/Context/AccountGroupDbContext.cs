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

        public List<AccountGroup> findGroupsByNameNoTracking(string name)
        {
            return getItemDb().AsNoTracking()
                .Where(acc => acc.Name == name)
                .ToList();
        }

        public bool isDefaultGroupExists()
        {
            return getItemDb().Where(acc => acc.Name == "Default Group").Count() > 0;
        }

        public AccountGroup getDefaultGroup()
        {
            return getItemDb().Where(acc => acc.Name == "Default Group").FirstOrDefault();
        }


        public string create(AccountGroup item)
        {
            var result = AccountGroupDbContext.getInstance().findGroupsByNameNoTracking(item.Name);
            if (result != null && result.Count > 0)
            {
                return "This category name is already used by other account. Please enter another category name.";
            }

            getItemDb().Add(item);
            db.SaveChanges();
            return null;
        }

        public string edit(AccountGroup item)
        {
            var accountGroup = findGroupByID(item.AccountGroupID);
            item.AccessibleCategories = String.Join(",", item.getAccessibleCategoryList().ToArray());
            accountGroup.AccessibleCategories = item.AccessibleCategories;

            var local = getItemDb()
                            .Local
                            .FirstOrDefault(f => f.AccountGroupID == item.AccountGroupID);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }

            db.Entry(accountGroup).State = EntityState.Modified;

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