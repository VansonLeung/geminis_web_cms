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
                context = new ConstantDbContext();
            }
            return context;
        }


        // initializations 
        



        // methods

        public List<Constant> find()
        {
            using (var db = new BaseDbContext())
            {
                return db.constantDb
                .ToList();
            }
        }
        public Constant findByID(int ID)
        {
            using (var db = new BaseDbContext())
            {
                return db.constantDb
                .Where(acc => acc.ConstantID == ID)
                .FirstOrDefault();
            }
        }

        public Constant findByKeyNoTracking(string Key)
        {
            using (var db = new BaseDbContext())
            {
                return db.constantDb.AsNoTracking()
                .Where(acc => acc.Key == Key)
                .FirstOrDefault();
            }
        }

        public Constant findActiveByKeyNoTracking(string Key)
        {
            using (var db = new BaseDbContext())
            {
                return db.constantDb.AsNoTracking()
                .Where(acc => acc.Key == Key && acc.isActive == true)
                .FirstOrDefault();
            }
        }

        public List<Constant> findActiveNoTracking()
        {
            using (var db = new BaseDbContext())
            {
                return db.constantDb.AsNoTracking()
                .Where(acc => acc.isActive == true)
                .ToList();
            }
        }


        public string create(Constant item)
        {
            using (var db = new BaseDbContext())
            {
                var result = ConstantDbContext.getInstance().findByKeyNoTracking(item.Key);
                if (result != null)
                {
                    return "This Constant Key already exists. Please enter another Constant Key.";
                }

                db.constantDb.Add(item);
                db.SaveChanges();
                return null;
            }
        }

        public string edit(Constant item)
        {
            using (var db = new BaseDbContext())
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

                var local = db.constantDb
                                .Local
                                .FirstOrDefault(f => f.ConstantID == item.ConstantID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(constant).State = EntityState.Modified;

                constant.Value = item.Value;

                db.SaveChanges();
                return null;
            }
        }

        public string delete(Constant item)
        {
            using (var db = new BaseDbContext())
            {
                db.Entry(item).State = EntityState.Deleted;
                db.SaveChanges();
                return null;
            }
        }






        public bool ALLOW_EDIT_AFTER_PUBLISH()
        {
            return ConstantDbContext.getInstance().findActiveByKeyNoTracking("ALLOW_EDIT_AFTER_PUBLISH") != null;
        }
    }
}