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
    public class ConstantDbContext
    {
        // singleton

        private static ConstantDbContext context;

        public static ConstantDbContext getInstance()
        {
            if (context == null)
            {
                return new ConstantDbContext();
            }
            return context;
        }


        // initializations 

        private BaseDbContext db = BaseDbContext.getInstance();

        protected virtual DbSet<Constant> getItemDb()
        {
            return db.constantDb;
        }



        // methods

        public List<Constant> find()
        {
            return getItemDb()
                .ToList();
        }
        public Constant findByID(int ID)
        {
            return getItemDb()
                .Where(acc => acc.ConstantID == ID)
                .FirstOrDefault();
        }

        public Constant findByKeyNoTracking(string Key)
        {
            return getItemDb().AsNoTracking()
                .Where(acc => acc.Key == Key)
                .FirstOrDefault();
        }

        public Constant findActiveByKeyNoTracking(string Key)
        {
            return getItemDb().AsNoTracking()
                .Where(acc => acc.Key == Key && acc.isActive == true)
                .FirstOrDefault();
        }

        public List<Constant> findActiveNoTracking()
        {
            return getItemDb().AsNoTracking()
                .Where(acc => acc.isActive == true)
                .ToList();
        }


        public string create(Constant item)
        {
            var result = ConstantDbContext.getInstance().findByKeyNoTracking(item.Key);
            if (result != null)
            {
                return "This Constant Key already exists. Please enter another Constant Key.";
            }

            getItemDb().Add(item);
            db.SaveChanges();
            return null;
        }

        public string edit(Constant item)
        {
            var constant = findByID(item.ConstantID);

            var result = ConstantDbContext.getInstance().findByKeyNoTracking(item.Key);
            if (result != null)
            {
                if (item.ConstantID != result.ConstantID)
                {
                    return "This Constant Key already exists. Please enter another Constant Key.";
                }
            }

            var local = getItemDb()
                            .Local
                            .FirstOrDefault(f => f.ConstantID == item.ConstantID);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }

            db.Entry(constant).State = EntityState.Modified;

            db.SaveChanges();
            return null;
        }

        public string delete(Constant item)
        {
            getItemDb().Remove(item);
            db.SaveChanges();
            return null;
        }






        public bool ALLOW_EDIT_AFTER_PUBLISH()
        {
            return ConstantDbContext.getInstance().findActiveByKeyNoTracking("ALLOW_EDIT_AFTER_PUBLISH") != null;
        }
    }
}