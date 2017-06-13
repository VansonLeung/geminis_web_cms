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



        // methods

        public List<Category> findCategorysByParentID(int? parentItemID = null)
        {
            using (var db = new BaseDbContext())
            {
                if (parentItemID == null)
                {
                    return db.infrastructureCategoryDb
                        .Where(item => item.parentItemID == null)
                        .OrderBy(item => item.order)
                        .ToList();
                }
                return db.infrastructureCategoryDb
                    .Where(item => item.parentItemID == parentItemID)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .ToList();
            }
        }


        public List<Category> findCategorysExcept(int? itemID = null)
        {
            using (var db = new BaseDbContext())
            {
                if (itemID == null)
                {
                    return db.infrastructureCategoryDb
                        .OrderBy(item => item.order)
                        .ToList();
                }
                return db.infrastructureCategoryDb
                    .Where(item => item.ItemID != itemID)
                    .OrderBy(item => item.order)
                    .ToList();
            }
        }



        public List<Category> findCategorysInTreeExcept(int itemLevel = 0, int? itemID = null, int? parentItemID = null, List<Category> currentCategories = null)
        {
            using (var db = new BaseDbContext())
            {
                if (currentCategories == null)
                {
                    currentCategories = new List<Category>();
                }

                List<Category> categories = null;

                if (itemID != null)
                {
                    categories = db.infrastructureCategoryDb.AsNoTracking()
                        .Where(item => item.parentItemID == parentItemID)
                        .OrderBy(item => item.order)
                        .ToList();
                }
                else
                {
                    categories = db.infrastructureCategoryDb.AsNoTracking()
                        .Where(item => item.ItemID != itemID && item.parentItemID == parentItemID)
                        .OrderBy(item => item.order)
                        .ToList();
                }

                if (categories == null || categories.Count <= 0)
                {
                    return null;
                }

                foreach (Category cat in categories)
                {
                    currentCategories.Add(cat);
                    cat.itemLevel = itemLevel;

                    var subcategories = findCategorysInTreeExcept(itemLevel + 1, itemID, cat.ItemID);

                    if (subcategories != null)
                    {
                        currentCategories.AddRange(subcategories);
                    }
                }

                return currentCategories;
            }
        }


        public int findHowManyParentsForCategoryByItemID(int parents = 0, int? itemID = null)
        {
            using (var db = new BaseDbContext())
            {
                if (itemID == null)
                {
                    return parents;
                }

                var category = db.infrastructureCategoryDb
                    .Where(item => item.ItemID == itemID)
                    .FirstOrDefault();

                if (category == null)
                {
                    return parents;
                }

                parents += 1;

                return findHowManyParentsForCategoryByItemID(parents, category.parentItemID);
            }
        }


        public List<Category> findCategorysByParentIDAsNoTracking(int? parentItemID = null)
        {
            using (var db = new BaseDbContext())
            {
                if (parentItemID == null || parentItemID == 0)
                {
                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item => item.parentItemID == null)
                        .OrderBy(item => item.order)
                        .ToList();
                }
                return db.infrastructureCategoryDb
                    .Where(item => item.parentItemID == parentItemID)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .ToList();
            }
        }

        public List<Category> findActiveCategorysByParentIDAsNoTracking(int? parentItemID = null)
        {
            using (var db = new BaseDbContext())
            {
                if (parentItemID == null || parentItemID == 0)
                {
                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item => item.parentItemID == null
                        && item.isEnabled == true)
                        .OrderBy(item => item.order)
                        .ToList();
                }
                else if (parentItemID == -1)
                {
                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item => item.isEnabled == true)
                        .OrderBy(item => item.order)
                        .ToList();
                }
                return db.infrastructureCategoryDb
                    .Where(item => item.parentItemID == parentItemID
                        && item.isEnabled == true)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .ToList();
            }
        }

        public List<Category> findAllCategorysContentPagesAsNoTracking()
        {
            using (var db = new BaseDbContext())
            {
                var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();

                if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
                {
                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item => item.isContentPage == true)
                        .OrderBy(item => item.order)
                        .ToList();
                }

                if (SessionPersister.account != null)
                {
                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item => item.isContentPage == true &&
                        categories.Contains(item.ItemID))
                        .OrderBy(item => item.order)
                        .ToList();
                }

                return new List<Category>();
            }
        }

        public List<Category> findAllCategorysArticleListsAsNoTracking()
        {
            using (var db = new BaseDbContext())
            {
                if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
                {
                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item => item.isArticleList == true)
                        .OrderBy(item => item.order)
                        .ToList();
                }

                if (SessionPersister.account != null)
                {
                    var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();

                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item => item.isArticleList == true &&
                        categories.Contains(item.ItemID))
                        .OrderBy(item => item.order)
                        .ToList();
                }

                return new List<Category>();
            }
        }



        public List<Category> findAllCategorysAsNoTracking()
        {
            using (var db = new BaseDbContext())
            {
                if (SessionPersister.account != null && SessionPersister.account.isRoleSuperadmin())
                {
                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .OrderBy(item => item.order)
                        .ToList();
                }

                if (SessionPersister.account != null)
                {
                    var categories = SessionPersister.account.Group.getAccessibleCategoryListInt();

                    return db.infrastructureCategoryDb
                        .AsNoTracking()
                        .Where(item =>
                        categories.Contains(item.ItemID))
                        .OrderBy(item => item.order)
                        .ToList();
                }

                return new List<Category>();
            }
        }



        public List<Category> findAllEnabledCategorysByPermission(string role = null)
        {
            using (var db = new BaseDbContext())
            {
                if (role != null)
                {
                    if (role == "trading")
                    {
                        return db.infrastructureCategoryDb
                            .Where(item => item.isEnabled == true
                            && (item.isVisibleToTradingOnly == true
                            || item.isVisibleToMembersOnly == true
                            || item.isVisibleToVisitorOnly == true))
                            .OrderBy(item => item.order)
                            .Include(item => item.parentItem)
                            .ToList();
                    }
                    else if (role == "member")
                    {
                        return db.infrastructureCategoryDb
                            .Where(item => item.isEnabled == true
                            && (item.isVisibleToMembersOnly == true
                            || item.isVisibleToVisitorOnly == true))
                            .OrderBy(item => item.order)
                            .Include(item => item.parentItem)
                            .ToList();
                    }
                    else if (role == "visitor")
                    {
                        return db.infrastructureCategoryDb
                            .Where(item => item.isEnabled == true
                            && item.isVisibleToVisitorOnly == true
                            && item.isVisibleToMembersOnly == false
                            && item.isVisibleToTradingOnly == false)
                            .OrderBy(item => item.order)
                            .Include(item => item.parentItem)
                            .ToList();
                    }
                }
                return db.infrastructureCategoryDb
                    .Where(item => item.isEnabled == true
                    && item.isVisibleToVisitorOnly == true
                    && item.isVisibleToMembersOnly == false
                    && item.isVisibleToTradingOnly == false)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .ToList();
            }
        }


        public List<Category> findEnabledCategorysByParentID(int? parentItemID = null)
        {
            using (var db = new BaseDbContext())
            {
                if (parentItemID == null)
                {
                    return db.infrastructureCategoryDb
                        .Where(item => item.parentItemID == null && item.isEnabled == true)
                        .OrderBy(item => item.order)
                        .ToList();
                }
                return db.infrastructureCategoryDb
                    .Where(item => item.parentItemID == parentItemID && item.isEnabled == true)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .ToList();
            }
        }


        public Category findCategoryByID(int? itemID)
        {
            using (var db = new BaseDbContext())
            {
                if (itemID == null) return null;
                return db.infrastructureCategoryDb
                    .Where(item => item.ItemID == itemID)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .FirstOrDefault();
            }
        }


        public Category findCategoryByURL(string URL)
        {
            using (var db = new BaseDbContext())
            {
                if (URL == null) return null;
                return db.infrastructureCategoryDb
                    .Where(item => item.url == URL)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .FirstOrDefault();
            }
        }

        public Category findCategoryByIDNoTracking(int? itemID)
        {
            using (var db = new BaseDbContext())
            {
                if (itemID == null) return null;
                return db.infrastructureCategoryDb
                    .AsNoTracking()
                    .Where(item => item.ItemID == itemID)
                    .OrderBy(item => item.order)
                    .Include(item => item.parentItem)
                    .FirstOrDefault();
            }
        }

        public string create(Category category)
        {
            using (var db = new BaseDbContext())
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

                if (category.iconPath != null && category.iconPath.Equals("____EMPTY"))
                {
                    category.iconPath = null;
                }

                if (category.thumbPath != null && category.thumbPath.Equals("____EMPTY"))
                {
                    category.thumbPath = null;
                }

                if (category.imagePath != null && category.imagePath.Equals("____EMPTY"))
                {
                    category.imagePath = null;
                }

                AuditLogDbContext.getInstance().createAuditLogCategoryAction(category, AuditLogDbContext.ACTION_CREATE);

                db.infrastructureCategoryDb.Add(category);
                db.SaveChanges();
                return null;
            }
        }

        public string edit(Category category)
        {
            using (var db = new BaseDbContext())
            {
                if (category.parentItemID == category.ItemID)
                {
                    return "Parent Item ID should not be the same as its own item ID.";
                }

                var _category = findCategoryByIDNoTracking(category.ItemID);
                category.created_at = _category.created_at;

                var local = db.infrastructureCategoryDb
                                .Local
                                .FirstOrDefault(f => f.ItemID == category.ItemID);
                if (local != null)
                {
                    db.Entry(local).State = EntityState.Detached;
                }


                List<string> modified_fields = new List<string>();

                if (_category.url != category.url) { modified_fields.Add("url"); }
                if (_category.name_en != category.name_en) { modified_fields.Add("name_en"); }
                if (_category.name_zh != category.name_zh) { modified_fields.Add("name_zh"); }
                if (_category.name_cn != category.name_cn) { modified_fields.Add("name_cn"); }
                if (_category.iconPath != category.iconPath) { modified_fields.Add("iconPath"); }
                if (_category.thumbPath != category.thumbPath) { modified_fields.Add("thumbPath"); }
                if (_category.imagePath != category.imagePath) { modified_fields.Add("imagePath"); }
                if (_category.remarks != category.remarks) { modified_fields.Add("remarks"); }
                if (_category.pageClassName != category.pageClassName) { modified_fields.Add("pageClassName"); }
                if (_category.isEnabled != category.isEnabled) { modified_fields.Add("isEnabled"); }
                if (_category.isContentPage != category.isContentPage) { modified_fields.Add("isContentPage"); }
                if (_category.isArticleList != category.isArticleList) { modified_fields.Add("isArticleList"); }
                if (_category.isVisibleToVisitorOnly != category.isVisibleToVisitorOnly) { modified_fields.Add("isVisibleToVisitorOnly"); }
                if (_category.isVisibleToMembersOnly != category.isVisibleToMembersOnly) { modified_fields.Add("isVisibleToMembersOnly"); }
                if (_category.isVisibleToTradingOnly != category.isVisibleToTradingOnly) { modified_fields.Add("isVisibleToTradingOnly"); }
                if (_category.isHeaderMenu != category.isHeaderMenu) { modified_fields.Add("isHeaderMenu"); }
                if (_category.isHeaderMenuRight != category.isHeaderMenuRight) { modified_fields.Add("isHeaderMenuRight"); }
                if (_category.isFooterMenu != category.isFooterMenu) { modified_fields.Add("isFooterMenu"); }
                if (_category.isBottomMenu != category.isBottomMenu) { modified_fields.Add("isBottomMenu"); }
                if (_category.isShortcut != category.isShortcut) { modified_fields.Add("isShortcut"); }
                if (_category.isJumbotron != category.isJumbotron) { modified_fields.Add("isJumbotron"); }
                if (_category.isBanner != category.isBanner) { modified_fields.Add("isBanner"); }
                if (_category.pageShouldShowTopbarmenu != category.pageShouldShowTopbarmenu) { modified_fields.Add("pageShouldShowTopbarmenu"); }
                if (_category.pageShouldHideTopTitle != category.pageShouldHideTopTitle) { modified_fields.Add("pageShouldHideTopTitle"); }
                if (_category.pageShouldHideFromHorizontalMenu != category.pageShouldHideFromHorizontalMenu) { modified_fields.Add("pageShouldHideFromHorizontalMenu"); }
                if (_category.isUseNewsArticleDetailsTemplate != category.isUseNewsArticleDetailsTemplate) { modified_fields.Add("isUseNewsArticleDetailsTemplate"); }



                if (category.parentItemID < 0)
                {
                    category.parentItemID = null;
                }

                if (category.iconPath != null && category.iconPath.Equals("____EMPTY"))
                {
                    category.iconPath = null;
                }
                else if (category.iconPath == null)
                {
                    category.iconPath = _category.iconPath;
                }

                if (category.thumbPath != null && category.thumbPath.Equals("____EMPTY"))
                {
                    category.thumbPath = null;
                }
                else if (category.thumbPath == null)
                {
                    category.thumbPath = _category.thumbPath;
                }

                if (category.imagePath != null && category.imagePath.Equals("____EMPTY"))
                {
                    category.imagePath = null;
                }
                else if (category.imagePath == null)
                {
                    category.imagePath = _category.imagePath;
                }

                AuditLogDbContext.getInstance().createAuditLogCategoryAction(category, AuditLogDbContext.ACTION_EDIT, modified_fields);

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return null;
            }
        }

        public string delete(Category category, bool isRecursive)
        {
            using (var db = new BaseDbContext())
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

                AuditLogDbContext.getInstance().createAuditLogCategoryAction(category, AuditLogDbContext.ACTION_DELETE);

                db.Entry(category).State = EntityState.Deleted;
                db.SaveChanges();

                return null;
            }
        }
    }
}