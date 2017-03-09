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



        // methods

        public List<AccountGroup> findGroups()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountGroupDb
                .ToList();
            }
        }
        public AccountGroup findGroupByID(int ID)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountGroupDb
                .Where(acc => acc.AccountGroupID == ID)
                .FirstOrDefault();
            }
        }

        public List<AccountGroup> findGroupsByNameNoTracking(string name)
        {
            using (var db = new BaseDbContext())
            {
                return db.accountGroupDb
                .AsNoTracking()
                .Where(acc => acc.Name == name)
                .ToList();
            }
        }

        public bool isDefaultGroupExists()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountGroupDb
                .Where(acc => acc.Name == "Default Group").Count() > 0;
            }
        }

        public AccountGroup getDefaultGroup()
        {
            using (var db = new BaseDbContext())
            {
                return db.accountGroupDb
                .Where(acc => acc.Name == "Default Group").FirstOrDefault();
            }
        }


        public string create(AccountGroup item)
        {
            var result = AccountGroupDbContext.getInstance().findGroupsByNameNoTracking(item.Name);
            if (result != null && result.Count > 0)
            {
                return "This Account Group Name already exists. Please enter another Account Group Name.";
            }

            using (var db = new BaseDbContext())
            {
                db.accountGroupDb.Add(item);
                db.SaveChanges();
                return null;
            }
        }

        public string edit(AccountGroup item)
        {
            var accountGroup = findGroupByID(item.AccountGroupID);

            var result = AccountGroupDbContext.getInstance().findGroupsByNameNoTracking(item.Name);
            if (result != null && result.Count > 0)
            {
                foreach (var res in result)
                {
                    if (item.AccountGroupID != res.AccountGroupID)
                    {
                        return "This Account Group Name already exists. Please enter another Account Group Name.";
                    }
                }
            }

            item.AccessibleCategories = String.Join(",", item.getAccessibleCategoryList().ToArray());
            accountGroup.AccessibleCategories = item.AccessibleCategories;

            using (var db = new BaseDbContext())
            {
                var local = db.accountGroupDb
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
        }

        public string delete(AccountGroup item)
        {
            var accounts = AccountDbContext.getInstance().findAccountsByAccountGroup(item);

            if (accounts.Count > 0)
            {
                return "Cannot delete account group which exists account(s).";
            }

            using (var db = new BaseDbContext())
            {
                db.Entry(item).State = EntityState.Deleted;
                db.SaveChanges();
                return null;
            }
        }
    }
}