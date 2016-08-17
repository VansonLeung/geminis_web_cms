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
    public class InfrastructureCategoryDbContext
    {
        // singleton

        private static InfrastructureCategoryDbContext infrastructureCategoryDbContext;

        public static InfrastructureCategoryDbContext getInstance()
        {
            if (infrastructureCategoryDbContext == null)
            {
                infrastructureCategoryDbContext = new InfrastructureCategoryDbContext();
            }
            return infrastructureCategoryDbContext;
        }


        // initializations 

        private BaseDbContext db = new BaseDbContext();

        protected virtual DbSet<Category> getItemDb()
        {
            return db.infrastructureCategoryDb;
        }



        // methods

        public List<Category> findCategorysByParentID(int? parentItemID = null)
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


        public List<Category> findCategorysByParentIDAsNoTracking(int? parentItemID = null)
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

        public List<Category> findAllCategorysAsNoTracking()
        {
            return getItemDb()
                .AsNoTracking()
                .OrderBy(item => item.order)
                .ToList();
        }


        public List<Category> findEnabledCategorysByParentID(int? parentItemID = null)
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


        public Category findCategoryByID(int? itemID)
        {
            if (itemID == null) return null;
            return getItemDb()
                .Where(item => item.ItemID == itemID)
                .OrderBy(item => item.order)
                .Include(item => item.parentItem)
                .FirstOrDefault();
        }


        public string create(Category category)
        {
            var categorys = findCategorysByParentID(category.parentItemID);
            int maxOrder = 0;
            for (int i = 0; i < categorys.Count(); i++)
            {
                var item = categorys.ElementAt(i);
                if (item.order > maxOrder)
                {
                    maxOrder = item.order;
                }
            }
            category.order = maxOrder + 1;
            if (category.parentItemID < 0)
            {
                category.parentItemID = null;
            }
            getItemDb().Add(category);
            db.SaveChanges();
            return null;
        }

        public string edit(Category category)
        {
            if (category.parentItemID == category.ItemID)
            {
                return "Parent Item ID should not be the same as its own item ID.";
            }

            var local = getItemDb()
                            .Local
                            .FirstOrDefault(f => f.ItemID == category.ItemID);
            if (local != null)
            {
                db.Entry(local).State = EntityState.Detached;
            }

            if (category.parentItemID < 0)
            {
                category.parentItemID = null;
            }
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            return null;
        }

        public string delete(Category category, bool isRecursive)
        {
            if (isRecursive)
            {
               // var children = findCategorysByParentID(category.parentItemID);
               /// for (int i = children.Count() - 1; i >= 0; i--)
               // {
               //     delete(children.ElementAt(i), true);
               // }
            }

            getItemDb().Remove(category);
            db.SaveChanges();
            return null;
        }
    }
}