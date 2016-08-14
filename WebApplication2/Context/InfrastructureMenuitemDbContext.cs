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
    public class InfrastructureMenuitemDbContext
    {
        // singleton

        private static InfrastructureMenuitemDbContext infrastructureMenuitemDbContext;

        public static InfrastructureMenuitemDbContext getInstance()
        {
            if (infrastructureMenuitemDbContext == null)
            {
                infrastructureMenuitemDbContext = new InfrastructureMenuitemDbContext();
            }
            return infrastructureMenuitemDbContext;
        }


        // initializations 

        private BaseDbContext db = new BaseDbContext();

        protected virtual DbSet<Menuitem> getItemDb()
        {
            return db.infrastructureMenuitemDb;
        }



        // methods

        public List<Menuitem> findMenuItemsByParentID(int? parentItemID = null)
        {
            if (parentItemID == null)
            {
                return getItemDb()
                    .Where(item => item.parentItemID == null)
                    .OrderBy(item => item.order)
                    .ToList();
            }
            return getItemDb()
                .Where(item => item.parentItemID == parentItemID)
                .OrderBy(item => item.order)
                .Include(item => item.parentItem)
                .ToList();
        }


        public List<Menuitem> findMenuItemsByParentIDAsNoTracking(int? parentItemID = null)
        {
            if (parentItemID == null)
            {
                return getItemDb()
                    .AsNoTracking()
                    .Where(item => item.parentItemID == null)
                    .OrderBy(item => item.order)
                    .ToList();
            }
            return getItemDb()
                .Where(item => item.parentItemID == parentItemID)
                .OrderBy(item => item.order)
                .Include(item => item.parentItem)
                .ToList();
        }


        public List<Menuitem> findEnabledMenuItemsByParentID(int? parentItemID = null)
        {
            if (parentItemID == null)
            {
                return getItemDb()
                    .Where(item => item.parentItemID == null && item.isEnabled == true)
                    .OrderBy(item => item.order)
                    .ToList();
            }
            return getItemDb()
                .Where(item => item.parentItemID == parentItemID && item.isEnabled == true)
                .OrderBy(item => item.order)
                .Include(item => item.parentItem)
                .ToList();
        }


        public Menuitem findMenuItemByID(int? itemID, bool asNoTracking = false)
        {
            if (itemID == null) return null;

            return getItemDb()
                .Where(item => item.ItemID == itemID)
                .OrderBy(item => item.order)
                .Include(item => item.parentItem)
                .FirstOrDefault();
        }


        public string create(Menuitem menuitem)
        {
            var menuitems = findMenuItemsByParentID(menuitem.parentItemID);
            int maxOrder = 0;
            for (int i = 0; i < menuitems.Count(); i++)
            {
                var item = menuitems.ElementAt(i);
                if (item.order > maxOrder)
                {
                    maxOrder = item.order;
                }
            }
            menuitem.order = maxOrder + 1;
            if (menuitem.parentItemID < 0)
            {
                menuitem.parentItemID = null;
            }
            getItemDb().Add(menuitem);
            db.SaveChanges();
            return null;
        }

        public string edit(Menuitem menuitem)
        {
            if (menuitem.parentItemID == menuitem.ItemID)
            {
                return "Parent Item ID should not be the same as its own item ID.";
            }

            var local = getItemDb()
                            .Local
                            .FirstOrDefault(f => f.ItemID == menuitem.ItemID);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }

            if (menuitem.parentItemID < 0)
            {
                menuitem.parentItemID = null;
            }
            db.Entry(menuitem).State = EntityState.Modified;
            db.SaveChanges();
            return null;
        }

        public string delete(Menuitem menuitem, bool isRecursive)
        {
            if (isRecursive)
            {
               // var children = findMenuItemsByParentID(menuitem.parentItemID);
               /// for (int i = children.Count() - 1; i >= 0; i--)
               // {
               //     delete(children.ElementAt(i), true);
               // }
            }

            getItemDb().Remove(menuitem);
            db.SaveChanges();
            return null;
        }
    }
}