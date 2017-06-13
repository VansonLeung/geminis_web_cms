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
            }
            AuditLogDbContext.getInstance().createAuditLogConstantAction(item, AuditLogDbContext.ACTION_CREATE);
            return null;
        }

        public string edit(Constant item)
        {
            List<string> modified_fields = new List<string>();

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
                    if (local.Key != item.Key) { modified_fields.Add("Key"); }
                    if (local.Value != item.Value) { modified_fields.Add("Value"); }
                    if (local.isActive != item.isActive) { modified_fields.Add("isActive"); }
                    if (local.Desc != item.Desc) { modified_fields.Add("Desc"); }

                    db.Entry(local).State = EntityState.Detached;
                }

                db.Entry(constant).State = EntityState.Modified;

                constant.Value = item.Value;
                constant.Key = item.Key;
                constant.isActive = item.isActive;

                db.SaveChanges();
            }
            AuditLogDbContext.getInstance().createAuditLogConstantAction(item, AuditLogDbContext.ACTION_EDIT, modified_fields);
            return null;
        }

        public string delete(Constant item)
        {
            using (var db = new BaseDbContext())
            {
                db.Entry(item).State = EntityState.Deleted;
                db.SaveChanges();
            }
            AuditLogDbContext.getInstance().createAuditLogConstantAction(item, AuditLogDbContext.ACTION_DELETE);
            return null;
        }






        public bool ALLOW_EDIT_AFTER_PUBLISH()
        {
            return ConstantDbContext.getInstance().findActiveByKeyNoTracking("ALLOW_EDIT_AFTER_PUBLISH") != null;
        }
    }
}