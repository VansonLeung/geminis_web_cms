using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace Frontend.ViewModels
{
    public class BaseViewModel
    {
        public string GetConstant(string Key)
        {
            if (Key == null)
            {
                return null;
            }

            foreach (var constant in constants)
            {
                if (constant.Key.Equals(Key))
                {
                    return constant.Value;
                }
            }
            return null;
        }

        public string GetQuery(string Key)
        {
            if (Key == null)
            {
                return null;
            }

            foreach (var constant in queries)
            {
                if (constant.Key.Equals(Key))
                {
                    return constant.Value;
                }
            }
            return null;
        }

        public List<Constant> constants { get; set; }
        public List<Constant> queries { get; set; }
        public Category headerData { get; set; }
        public List<Category> breadcrumbData { get; set; }
        public Category footerData { get; set; }
        public List<Menu> headerMenu { get; set; }
        public List<Menu> footerMenu { get; set; }
        public Category category { get; set; }
        public Content content { get; set; }
        public Current current { get; set; }
        public Lang lang { get; set; }
        public List list { get; set; }
        public Menu menu { get; set; }
        public Sideitem sideitem { get; set; }

        public bool isError { get; set; } 
        public int errorCode { get; set; }
        public string errorMessage { get; set; }



        public static List<Menu> createSubmenu(
            int categoryItemID,
            Lang lang,
            bool isHeaderMenu,
            bool isFooterMenu,
            bool isShortcut,
            bool isBanner,
            bool isJumbotron)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                if (isHeaderMenu && !_cat.isHeaderMenu)
                {
                    continue;
                }

                if (isFooterMenu && !_cat.isFooterMenu)
                {
                    continue;
                }

                if (isShortcut && !_cat.isShortcut)
                {
                    continue;
                }

                if (isBanner && !_cat.isBanner)
                {
                    continue;
                }

                if (isJumbotron && !_cat.isJumbotron)
                {
                    continue;
                }

                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null);
                item.category = new Category(_cat, lang);

                item.submenu = createSubmenu(item.category.categoryItemID, lang, 
                    isHeaderMenu,
                    isFooterMenu,
                    isShortcut,
                    isBanner,
                    isJumbotron);

                menuitems.Add(item);
            }

            return menuitems;
        }


        public static List<Menu> createHeaderMenu(int categoryItemID, Lang lang)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                if (!_cat.isHeaderMenu)
                {
                    continue;
                }

                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null);
                item.category = new Category(_cat, lang);

                item.submenu = createSubmenu(item.category.categoryItemID, lang, 
                    true,
                    false,
                    false,
                    false,
                    false);

                menuitems.Add(item);
            }

            return menuitems;
        }

        public static List<Menu> createFooterMenu(int categoryItemID, Lang lang)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null);
                item.category = new Category(_cat, lang);

                item.submenu = createSubmenu(item.category.categoryItemID, lang,
                    false,
                    true,
                    false,
                    false,
                    false);

                menuitems.Add(item);
            }

            return menuitems;
        }


        public static Category getCategoryRecursively(int categoryItemID, Lang lang)
        {
            Category category = null;
            var cat = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findCategoryByIDNoTracking(categoryItemID);
            if (cat != null)
            {
                category = new Category(cat, lang);
                category.categoryParent = getCategoryRecursively(category.categoryParentItemID, lang);
            }
            return category;
        }

        public static List<Category> convertCategoryRecursiveToList(List<Category> categories, Category category)
        {
            if (category != null)
            {
                if (category.categoryParent != null)
                {
                    categories = convertCategoryRecursiveToList(categories, category.categoryParent);
                }
                categories.Add(category);
            }
            return categories;
        }

        public static List<Category> createBreadcrumbData(int categoryItemID, Lang lang)
        {
            Category category = getCategoryRecursively(categoryItemID, lang);
            List<Category> breadcrumbData = new List<Category>();
            breadcrumbData = convertCategoryRecursiveToList(breadcrumbData, category);
            return breadcrumbData;
        }

        public static BaseViewModel make(string locale, string category, string id, HttpRequestBase request)
        {
            // locale

            if (locale == null)
            {
                locale = "zh-HK";
            }

            string language = "zh";
            string culture = "HK";

            try
            {
                language = locale.Split('-')[0];
                culture = locale.Split('-')[1];
            }
            catch (Exception e)
            {

            }

            BaseViewModel vm = new BaseViewModel();
            vm.lang = new Lang();
            vm.lang.locale = locale;
            vm.lang.lang = language;
            vm.lang.culture = culture;


            // constants


            vm.constants = new List<Constant>();
            var constants = WebApplication2.Context.ConstantDbContext.getInstance().findActiveNoTracking();

            foreach (var constant in constants)
            {
                vm.constants.Add(constant);
            }


            // queries

            vm.queries = new List<Constant>();
            var keys = request.QueryString.Keys;
            for (var i = 0; i < keys.Count; i++)
            {
                var val = request.QueryString[keys[i]];
                Constant constant = new Constant();
                constant.Key = keys[i];
                constant.Value = val;
                constant.isActive = true;
                vm.queries.Add(constant);
            }



            // category

            var db = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance();
            var cat = db.findCategoryByURL(category);

            vm.category = new Category(cat, vm.lang);




            // header data

            vm.headerData = new Category(null, null);
            vm.headerData.title = "Geminis";


            // header menu

            vm.headerMenu = createHeaderMenu(0, vm.lang);

            // footer menu

            vm.footerMenu = createFooterMenu(0, vm.lang);

            // breadcrumb data

            vm.breadcrumbData = createBreadcrumbData(vm.category.categoryItemID, vm.lang);



            // content

            vm.content = null;

            if (cat != null)
            {
                if (cat.isArticleList)
                {
                    vm.category.type = "ArticleList";

                    WebApplication2.Models.ArticlePublished articlePublished = null;

                    if (id != null)
                    {
                        var idInt = int.Parse(id);
                        articlePublished = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getArticlePublishedByBaseArticleID(idInt, vm.lang.lang);
                    }

                    if (articlePublished != null)
                    {
                        vm.content = new Content();
                        vm.content.name = articlePublished.Name;
                        vm.content.desc = articlePublished.Desc;
                        vm.content.slug = articlePublished.Slug;
                        vm.content.link = new Link(vm.lang.locale, cat.getUrl(), articlePublished.BaseArticleID + "");
                        vm.content.link.is_absolute = false;
                        vm.content.link.is_external = false;
                        vm.content.type = "Article";
                    }
                    else
                    {
                        vm.content = new Content();
                        vm.content.articleList = new List<Listitem>();
                        var articleList = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getArticlesPublishedByCategory(cat, vm.lang.lang);
                        foreach (var article in articleList)
                        {
                            Listitem item = new Listitem();
                            item.name = article.Name;
                            item.summary = article.Excerpt;
                            vm.content.link = new Link(vm.lang.locale, cat.getUrl(), article.BaseArticleID + "");
                            item.link.is_absolute = false;
                            item.link.is_external = false;
                        }
                        vm.content.type = "ArticleList";
                    }
                }

                else if (cat.isContentPage)
                {
                    vm.category.type = "ContentPage";

                    WebApplication2.Models.ArticlePublished contentPage = null;
                    var contentPages = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getArticlesPublishedByCategory(cat, vm.lang.lang);
                    if (contentPages.Count > 0)
                    {
                        contentPage = contentPages[0];
                    }

                    if (contentPage != null)
                    {
                        vm.content = new Content();
                        vm.content.name = contentPage.Name;
                        vm.content.desc = contentPage.Desc;
                        vm.content.slug = contentPage.Slug;
                        vm.content.link = new Link(vm.lang.locale, cat.getUrl(), contentPage.BaseArticleID + "");
                        vm.content.link.is_absolute = false;
                        vm.content.link.is_external = false;
                        vm.content.type = "ContentPage";
                    }
                }

                else

                {
                    vm.content = null;
                }
            }

            if (vm.content == null)
            {
                vm.category.isNoContent = true;
                vm.isError = true;
                vm.errorCode = 404;
                vm.errorMessage = "Error: Page not found";
            }
            else
            {
                vm.category.isNoContent = false;
            }


            return vm;
        }
    }
}