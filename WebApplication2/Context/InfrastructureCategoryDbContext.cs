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

        private BaseDbContext db = BaseDbContext.getInstance();

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
            if (parentItemID == null || parentItemID == 0)
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

        public List<Category> findActiveCategorysByParentIDAsNoTracking(int? parentItemID = null)
        {
            if (parentItemID == null || parentItemID == 0)
            {
                return getItemDb()
                    .AsNoTracking()
                    .Where(item => item.parentItemID == null 
                    && item.isEnabled == true)
                    .OrderBy(item => item.order)
                    .ToList();
            }
            return getItemDb()
                .Where(item => item.parentItemID == parentItemID
                    && item.isEnabled == true)
                .OrderBy(item => item.order)
                .Include(item => item.parentItem)
                .ToList();
        }

        public List<Category> findAllCategorysContentPagesAsNoTracking()
        {
            var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();

            if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
            {
                return getItemDb()
                    .AsNoTracking()
                    .Where(item => item.isContentPage == true)
                    .OrderBy(item => item.order)
                    .ToList();
            }

            if (SessionPersister.account != null)
            {
                return getItemDb()
                    .AsNoTracking()
                    .Where(item => item.isContentPage == true &&
                    categories.Contains(item.ItemID))
                    .OrderBy(item => item.order)
                    .ToList();
            }

            return new List<Category>();
        }

        public List<Category> findAllCategorysArticleListsAsNoTracking()
        {
            if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
            {
                return getItemDb()
                    .AsNoTracking()
                    .Where(item => item.isArticleList == true)
                    .OrderBy(item => item.order)
                    .ToList();
            }

            if (SessionPersister.account != null)
            {
                var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();

                return getItemDb()
                    .AsNoTracking()
                    .Where(item => item.isArticleList == true &&
                    categories.Contains(item.ItemID))
                    .OrderBy(item => item.order)
                    .ToList();
            }

            return new List<Category>();
        }



        public List<Category> findAllCategorysAsNoTracking()
        {
            if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
            {
                return getItemDb()
                    .AsNoTracking()
                    .OrderBy(item => item.order)
                    .ToList();
            }

            if (SessionPersister.account != null)
            {
                var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();

                return getItemDb()
                    .AsNoTracking()
                    .Where(item => 
                    categories.Contains(item.ItemID))
                    .OrderBy(item => item.order)
                    .ToList();
            }

            return new List<Category>();
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


        public Category findCategoryByURL(string URL)
        {
            if (URL == null) return null;
            return getItemDb()
                .Where(item => item.url == URL)
                .OrderBy(item => item.order)
                .Include(item => item.parentItem)
                .FirstOrDefault();
        }

        public Category findCategoryByIDNoTracking(int? itemID)
        {
            if (itemID == null) return null;
            return getItemDb()
                .AsNoTracking()
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

            if (category.imagePath != null && category.imagePath.Equals("____EMPTY"))
            {
                category.imagePath = null;
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

            var _category = findCategoryByIDNoTracking(category.ItemID);
            category.created_at = _category.created_at;

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

            if (category.imagePath != null && category.imagePath.Equals("____EMPTY"))
            {
                category.imagePath = null;
            }
            else if (category.imagePath == null)
            {
                category.imagePath = _category.imagePath;
            }

            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            return null;
        }

        public string delete(Category category, bool isRecursive)
        {
            var articles = ArticleDbContext.getInstance().findArticlesByCategoryID(category.ItemID);
            var contentPages = ContentPageDbContext.getInstance().findArticlesByCategoryID(category.ItemID);

            if (articles.Count > 0 || contentPages.Count > 0)
            {
                return "Could not delete this category when it is linked with any article / content page.";
            }

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