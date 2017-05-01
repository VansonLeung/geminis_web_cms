using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Context;
using WebApplication2.Models;
using static WebApplication2.Controllers.BaseController;

namespace WebApplication2.ViewModels.Include
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

        public List<Constant> GetQueries()
        {
            return queries;
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
        public ViewCategory headerData { get; set; }
        public List<ViewCategory> breadcrumbData { get; set; }
        public ViewCategory footerData { get; set; }
        public List<Menu> headerMenu { get; set; }
        public List<Menu> headerMenuRight { get; set; }
        public List<Menu> footerMenu { get; set; }
        public List<Menu> bottomMenu { get; set; }
        public List<Menu> jumbotronMenu { get; set; }
        public List<Menu> shortcutMenu { get; set; }
        public ViewCategory category { get; set; }
        public ViewContent content { get; set; }
        public ViewContent nextContent { get; set; }
        public ViewContent prevContent { get; set; }
        public Current current { get; set; }
        public Lang lang { get; set; }
        public List list { get; set; }
        public Menu menu { get; set; }
        public Sideitem sideitem { get; set; }
        public List<Menu> topbarMenu { get; set; }

        public bool isError { get; set; } 
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public string currentYear { get; set; }

        public List<string> topWarningMessages { get; set; }


        public static List<Menu> createSubmenu(
            int categoryItemID,
            Lang lang,
            bool isHeaderMenu = false,
            bool isHeaderMenuRight = false,
            bool isFooterMenu = false,
            bool isShortcut = false,
            bool isBanner = false,
            bool isJumbotron = false,
            bool isBottomMenu = false)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                if (isHeaderMenu && !_cat.isHeaderMenu)
                {
                    continue;
                }

                if (isHeaderMenuRight && !_cat.isHeaderMenuRight)
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

                if (isBottomMenu && !_cat.isBottomMenu)
                {
                    continue;
                }

                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null, null);
                item.category = new ViewCategory(_cat, lang);

                item.submenu = createSubmenu(item.category.categoryItemID, lang, 
                    isHeaderMenu,
                    isHeaderMenuRight,
                    isFooterMenu,
                    isShortcut,
                    isBanner,
                    isJumbotron,
                    isBottomMenu);


                var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategory(_cat, lang.lang);
                if (hasArticles)
                {
                    item.is_has_published_content = true;
                }
                else if (item.submenu != null)
                {
                    foreach (Menu m in item.submenu)
                    {
                        if (m.is_has_published_content)
                        {
                            item.link = m.link;
                            break;
                        }
                    }
                }

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

                if (_cat.isHeaderMenuRight)
                {
                    continue;
                }

                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null, null);
                item.category = new ViewCategory(_cat, lang);

                item.submenu = createSubmenu(item.category.categoryItemID, lang,
                    true,
                    false,
                    false,
                    false,
                    false);


                var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategory(_cat, lang.lang);
                if (hasArticles)
                {
                    item.is_has_published_content = true;
                }
                else if (item.submenu != null)
                {
                    foreach (Menu m in item.submenu)
                    {
                        if (m.is_has_published_content)
                        {
                            item.link = m.link;
                            break;
                        }
                    }
                }


                menuitems.Add(item);
            }

            return menuitems;
        }
        public static List<Menu> createHeaderMenuRight(int categoryItemID, Lang lang)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                if (!_cat.isHeaderMenuRight)
                {
                    continue;
                }

                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null, null);
                item.category = new ViewCategory(_cat, lang);

                item.submenu = createSubmenu(item.category.categoryItemID, lang,
                    true,
                    false,
                    false,
                    false,
                    false);


                var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategory(_cat, lang.lang);
                if (hasArticles)
                {
                    item.is_has_published_content = true;
                }
                else if (item.submenu != null)
                {
                    foreach (Menu m in item.submenu)
                    {
                        if (m.is_has_published_content)
                        {
                            item.link = m.link;
                            break;
                        }
                    }
                }


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
                item.link = new Link(lang.locale, _cat.getUrl(), null, null);
                item.category = new ViewCategory(_cat, lang);

                if (_cat.isFooterMenu)
                {
                    item.submenu = createSubmenu(item.category.categoryItemID, lang,
                        false,
                        true,
                        false,
                        false,
                        false);


                    var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategory(_cat, lang.lang);
                    if (hasArticles)
                    {
                        item.is_has_published_content = true;
                    }
                    else if (item.submenu != null)
                    {
                        foreach (Menu m in item.submenu)
                        {
                            if (m.is_has_published_content)
                            {
                                item.link = m.link;
                                break;
                            }
                        }
                    }


                    menuitems.Add(item);
                }
            }

            return menuitems;
        }

        public static List<Menu> createShortcutMenu(int categoryItemID, Lang lang)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null, null);
                item.category = new ViewCategory(_cat, lang);

                if (_cat.isShortcut)
                {
                    item.submenu = createSubmenu(item.category.categoryItemID, lang,
                        false,
                        false,
                        true,
                        false,
                        false);


                    var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategory(_cat, lang.lang);
                    if (hasArticles)
                    {
                        item.is_has_published_content = true;
                    }
                    else if (item.submenu != null)
                    {
                        foreach (Menu m in item.submenu)
                        {
                            if (m.is_has_published_content)
                            {
                                item.link = m.link;
                                break;
                            }
                        }
                    }


                    menuitems.Add(item);
                }
            }

            return menuitems;
        }

        public static List<Menu> createJumbotronMenu(int categoryItemID, Lang lang)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null, null);
                item.category = new ViewCategory(_cat, lang);

                if (_cat.isJumbotron)
                {
                    item.submenu = createSubmenu(item.category.categoryItemID, lang,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true);
                    
                    WebApplication2.Models.ArticlePublished contentPage = null;
                    var contentPages = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getArticlesPublishedByCategory(_cat, lang.lang);
                    if (contentPages.Count > 0)
                    {
                        contentPage = contentPages[0];
                    }

                    if (contentPage != null)
                    {
                        item.desc = contentPage.Desc;
                    }


                    var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategory(_cat, lang.lang);
                    if (hasArticles)
                    {
                        item.is_has_published_content = true;
                    }
                    else if (item.submenu != null)
                    {
                        foreach (Menu m in item.submenu)
                        {
                            if (m.is_has_published_content)
                            {
                                item.link = m.link;
                                break;
                            }
                        }
                    }


                    menuitems.Add(item);
                }
            }

            return menuitems;
        }


        public static List<Menu> createBottomMenu(int categoryItemID, Lang lang)
        {
            List<Menu> menuitems = new List<Menu>();

            var rootCategories = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findActiveCategorysByParentIDAsNoTracking(categoryItemID);
            foreach (var _cat in rootCategories)
            {
                Menu item = new Menu();
                item.name = _cat.GetName(lang.lang);
                item.link = new Link(lang.locale, _cat.getUrl(), null, null);
                item.category = new ViewCategory(_cat, lang);

                if (_cat.isBottomMenu)
                {
                    item.submenu = createSubmenu(item.category.categoryItemID, lang,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true);


                    var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategory(_cat, lang.lang);
                    if (hasArticles)
                    {
                        item.is_has_published_content = true;
                    }
                    else if (item.submenu != null)
                    {
                        foreach (Menu m in item.submenu)
                        {
                            if (m.is_has_published_content)
                            {
                                item.link = m.link;
                                break;
                            }
                        }
                    }


                    menuitems.Add(item);
                }
            }

            return menuitems;
        }


        public static ViewCategory getCategoryRecursively(int categoryItemID, Lang lang)
        {
            ViewCategory category = null;
            var cat = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findCategoryByIDNoTracking(categoryItemID);
            if (cat != null)
            {
                category = new ViewCategory(cat, lang);
                category.categoryParent = getCategoryRecursively(category.categoryParentItemID, lang);
            }
            return category;
        }

        public static List<ViewCategory> convertCategoryRecursiveToList(List<ViewCategory> categories, ViewCategory category)
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

        public static List<ViewCategory> createBreadcrumbData(int categoryItemID, Lang lang)
        {
            ViewCategory category = getCategoryRecursively(categoryItemID, lang);
            List<ViewCategory> breadcrumbData = new List<ViewCategory>();
            breadcrumbData = convertCategoryRecursiveToList(breadcrumbData, category);


            for (var k = breadcrumbData.Count - 1; k >= 0; k--)
            {
                var cate = breadcrumbData[k];
                var _cat = cate.categoryItemID;
                var hasArticles = WebApplication2.Context.ArticlePublishedDbContext.getInstance().hasArticlesPublishedByCategoryID(_cat, lang.lang);
                if (!hasArticles && breadcrumbData.Count - 1 > k)
                {
                    cate.link = breadcrumbData[k + 1].link;
                }
            }


            if (breadcrumbData.Count > 0)
            {
                var cat = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance().findCategoryByURL("home");
                if (cat != null)
                {
                    ViewCategory home = new ViewCategory();
                    home.link = new Link(lang.locale, null, null, null);
                    home.title = cat.GetName(lang.lang);
                    breadcrumbData.Insert(0, home);
                }
            }
            return breadcrumbData;
        }

        public static BaseViewModel make(string locale, string category, string id, HttpRequestBase request, BaseControllerSession session)
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

            if (culture != null && culture == "CN")
            {
                language = "cn";
            }

            if (culture != null && culture == "HK")
            {
                language = "zh";
            }

            if (culture != null && culture == "TW")
            {
                language = "zh";
            }

            BaseViewModel vm = new BaseViewModel();
            vm.lang = new Lang();
            vm.lang.locale = locale;
            vm.lang.lang = language;
            vm.lang.culture = culture;

            vm.currentYear = DateTime.Now.Year.ToString();

            // sessions
            vm.current = new Current(session, null, null);




            // top warning

            vm.topWarningMessages = new List<string>();
            var systemMaintenanceNotifications = SystemMaintenanceNotificationDbContext.getInstance().findAllActivatedNotifications();
            foreach (var item in systemMaintenanceNotifications)
            {
                vm.topWarningMessages.Add(item.GetDesc(language));
            }

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

            if (vm.GetQuery("stock_code") == null)
            {
                Constant constant = new Constant();
                constant.Key = "stock_code";
                constant.Value = "00001";
                constant.isActive = true;
                vm.queries.Add(constant);
            }


            int articlelist_page = 1;
            int articlelist_size = 10;

            foreach (Constant constant in vm.queries)
            {
                if (constant.Key == "page")
                {
                    articlelist_page = int.Parse(constant.Value);
                }
                if (constant.Key == "size")
                {
                    articlelist_size = int.Parse(constant.Value);
                }
            }



            // category

            var db = WebApplication2.Context.InfrastructureCategoryDbContext.getInstance();

            WebApplication2.Models.Infrastructure.Category cat = null;


            // header data

            vm.headerData = new ViewCategory(null, null);
            vm.headerData.title = "Geminis";


            // header menu

            vm.headerMenu = createHeaderMenu(0, vm.lang);

            vm.headerMenuRight = createHeaderMenuRight(0, vm.lang);

            // footer menu

            vm.footerMenu = createFooterMenu(0, vm.lang);


            // bottom menu

            vm.bottomMenu = createBottomMenu(0, vm.lang);


            // shortcut menu

            vm.shortcutMenu = createShortcutMenu(0, vm.lang);


            // jumbotron menu

            vm.jumbotronMenu = createJumbotronMenu(0, vm.lang);




            if (category != null && locale != null)
            {

                cat = db.findCategoryByURL(category);

                vm.category = new ViewCategory(cat, vm.lang);



                // breadcrumb data

                vm.breadcrumbData = createBreadcrumbData(vm.category.categoryItemID, vm.lang);


                // top bar menu

                if (cat.pageShouldShowTopbarmenu && cat.parentItemID.HasValue)
                {
                    vm.topbarMenu = createSubmenu(cat.parentItemID.Value, vm.lang, false,false,false,false,false);
                }

                if (vm.topbarMenu != null && vm.topbarMenu.Count <= 0)
                {
                    vm.topbarMenu = null;
                }


            }


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
                        articlePublished = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getArticlePublishedBySlugAndCategoryID(cat.ItemID, id, vm.lang.lang);
                    }

                    if (articlePublished != null)
                    {
                        vm.content = new ViewContent();
                        vm.content.name = articlePublished.Name;
                        vm.content.desc = articlePublished.Desc;
                        vm.content.slug = articlePublished.Slug;
                        vm.content.link = new Link(vm.lang.locale, cat.getUrl(), null, articlePublished.Slug);
                        vm.content.link.is_absolute = false;
                        vm.content.link.is_external = false;
                        vm.content.type = "Article";
                        vm.content.datetime = articlePublished.datePublished;
                        vm.content.datetime_representation = articlePublished.getDatePublished();

                        var nextArticlePublished = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getNextArticlePublishedBySlugAndCategoryID(cat.ItemID, id, vm.lang.lang);
                        var prevArticlePublished = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getPrevArticlePublishedBySlugAndCategoryID(cat.ItemID, id, vm.lang.lang);
                        
                        if (nextArticlePublished != null)
                        {
                            vm.nextContent = new ViewContent();
                            vm.nextContent.name = nextArticlePublished.Name;
                            vm.nextContent.slug = nextArticlePublished.Slug;
                        }

                        if (prevArticlePublished != null)
                        {
                            vm.prevContent = new ViewContent();
                            vm.prevContent.name = prevArticlePublished.Name;
                            vm.prevContent.slug = prevArticlePublished.Slug;
                        }
                    }
                    else
                    {
                        vm.content = new ViewContent();
                        vm.content.articleList = new List<Listitem>();
                        var articleList = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getArticlesPublishedByCategoryPaginated(cat, articlelist_size, articlelist_page, vm.lang.lang);
                        foreach (var article in articleList)
                        {
                            Listitem item = new Listitem();
                            item.name = article.Name;
                            item.summary = article.Excerpt;
                            item.link = new Link(vm.lang.locale, cat.getUrl(), null, article.Slug);
                            item.link.is_absolute = false;
                            item.link.is_external = false;
                            vm.content.articleList.Add(item);
                        }
                        vm.content.articleListTotal = WebApplication2.Context.ArticlePublishedDbContext.getInstance().getArticlesPublishedByCategoryTotalCount(cat, vm.lang.lang);
                        vm.content.articleListTotalPages = vm.content.articleListTotal / articlelist_size;
                        vm.content.articleListPageSize = articlelist_size;
                        vm.content.articleListCurrentPage = articlelist_page;
                        vm.content.articleListHasPrevPage = articlelist_page <= 1;
                        vm.content.articleListHasNextPage = articlelist_page >= vm.content.articleListTotalPages;
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
                        vm.content = new ViewContent();
                        vm.content.name = contentPage.Name;
                        vm.content.desc = contentPage.Desc;
                        vm.content.slug = contentPage.Slug;
                        vm.content.link = new Link(vm.lang.locale, cat.getUrl(), contentPage.BaseArticleID + "", null);
                        vm.content.link.is_absolute = false;
                        vm.content.link.is_external = false;
                        vm.content.type = "ContentPage";
                        vm.content.datetime = contentPage.datePublished;
                        vm.content.datetime_representation = contentPage.getDatePublished();
                    }
                }

                else

                {
                    vm.content = null;
                }
            }

            if (vm.content == null)
            {
                if (vm.category == null)
                {
                    vm.category = new ViewCategory();
                }

                vm.category.isNoContent = true;
                vm.isError = true;
                vm.errorCode = 404;
                vm.errorMessage = "Error: Page not found";
            }
            else
            {
                if (vm.category == null)
                {
                    vm.category = new ViewCategory();
                }

                vm.category.isNoContent = false;

                if (cat.pageClassName != null)
                {
                    vm.content.pageClassName = cat.pageClassName;
                }

                if (cat.isUseNewsArticleDetailsTemplate)
                {
                    vm.content.showArticleDetailsTemplate = true;
                }
                else
                {
                    vm.content.showArticleDetailsTemplate = false;
                }
            }


            return vm;
        }
    }
}