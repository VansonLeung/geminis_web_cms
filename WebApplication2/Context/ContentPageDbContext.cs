﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class ContentPageDbContext
    {
        // singleton

        private static ContentPageDbContext contentPageDbContext;

        public static ContentPageDbContext getInstance()
        {
            if (contentPageDbContext == null)
            {
                contentPageDbContext = new ContentPageDbContext();
            }
            return contentPageDbContext;
        }


        // initialization

        private BaseDbContext db = new BaseDbContext();

        protected DbSet<ContentPage> getArticleDb()
        {
            return db.contentPageDb;
        }






        // methods

        public List<ContentPage> findArticles()
        {
            return (getArticleDb()).Include(acc => acc.createdByAccount).ToList();
        }

        public List<ContentPage> findArticlesGroupByBaseVersion(string lang = "en")
        {
            return getArticleDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .OrderByDescending(acc => acc.modified_at)
                .Include(acc => acc.createdByAccount)
                .ToList();
        }


        public ContentPage findArticleByID(int articleID)
        {
            return getArticleDb().Where(acc => acc.ArticleID == articleID).FirstOrDefault();
        }


        public List<ContentPage> findArticlesRequestingApproval()
        {
            return getArticleDb().Where(acc =>
            acc.isRequestingApproval == true)
            .OrderByDescending(acc => acc.modified_at)
            .Include(acc => acc.createdByAccount)
            .ToList();
        }

        public List<ContentPage> findArticlesGroupByBaseVersionRequestingApproval(string lang = "en")
        {
            return getArticleDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .Where(acc => acc.isRequestingApproval == true)
                .OrderByDescending(acc => acc.modified_at)
                .Include(acc => acc.createdByAccount)
                .ToList();
        }

        public List<ContentPage> findArticlesGroupByBaseVersionApproved(string lang = "en")
        {
            return getArticleDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .Where(acc => acc.isApproved == true)
                .OrderByDescending(acc => acc.dateApproved)
                .Include(acc => acc.createdByAccount)
                .ToList();
        }


        public ContentPage findArticleByVersionAndLang(int baseArticleID, int version = 0, String lang = null)
        {
            if (lang == null)
            {
                return null;
            }

            if (version == 0)
            {
                return getArticleDb().Where(acc =>
                acc.BaseArticleID == baseArticleID &&
                acc.Lang == lang)
                .OrderByDescending(acc => acc.Version)
                .Include(acc => acc.createdByAccount)
                .FirstOrDefault();
            }
            else
            {
                return getArticleDb().Where(acc =>
                acc.BaseArticleID == baseArticleID &&
                acc.Version == version &&
                acc.Lang == lang).FirstOrDefault();
            }
        }

        public ContentPage findArticleByArticle(ContentPage article)
        {
            return null;
            //   return articleDb.Where(acc => acc.Username == account.Username && acc.Password == account.Password).FirstOrDefault();
        }


        public ContentPage findLatestArticleByBaseArticle(ContentPage article, String lang = null)
        {
            var targetBaseArticleID = article.BaseArticleID;
            var targetLang = article.Lang;

            var baseLatestArticle = getArticleDb().Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .Include(acc => acc.createdByAccount)
            .FirstOrDefault();


            if (baseLatestArticle != null)
            {
                return baseLatestArticle;
            }

            return null;
        }


        public ContentPage findLatestArticleByBaseArticleID(int baseArticleID, String lang = null)
        {
            var targetBaseArticleID = baseArticleID;
            var targetLang = lang;

            var baseLatestArticle = getArticleDb().Where(acc =>
            acc.BaseArticleID == baseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .Include(acc => acc.createdByAccount)
            .FirstOrDefault();


            if (baseLatestArticle != null)
            {
                return baseLatestArticle;
            }

            return null;
        }



        public List<ContentPage> findAllArticlesByBaseArticle(ContentPage article, String lang = null)
        {
            var articles = getArticleDb().Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .Include(acc => acc.createdByAccount)
            .ToList();

            return articles;
        }





        public List<ContentPage> findAllLocaleArticlesByBaseArticleAndVersion(ContentPage article, String Lang = "en")
        {
            var articles = getArticleDb().Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang != Lang &&
            acc.Version == article.Version
            )
            .Include(acc => acc.createdByAccount)
            .ToList();

            return articles;
        }




        public bool articleWithSameVersionAndLangAlreadyPresents(ContentPage article)
        {
            return findArticleByVersionAndLang(article.BaseArticleID, article.Version, article.Lang) != null;
        }








        

        // ARTICLE EDITOR ONLY



        public String tryCreateNewArticle(ContentPage article)
        {
            ContentPage latestArticle = null;

            if (article.BaseArticleID != 0)
            {
                if (String.IsNullOrEmpty(article.Lang))
                {
                    article.Lang = "en";
                }

                if (!article.Lang.Equals("en"))
                {
                    return tryCreateNewLocaleArticle(article);
                }

                latestArticle = findLatestArticleByBaseArticle(article, null);
                article.Version = latestArticle.Version + 1;
            }
            else
            {
                article.Version = 1;
            }

            if (String.IsNullOrEmpty(article.Lang))
            {
                article.Lang = "en";
            }

            if (articleWithSameVersionAndLangAlreadyPresents(article))
            {
                return "ContentPage already presents";
            }

            article.createdBy = SessionPersister.account.AccountID;
            getArticleDb().Add(article);
            db.SaveChanges();


            if (article.BaseArticleID == 0)
            {
                db.Entry(article).State = EntityState.Modified;
                article.BaseArticleID = article.ArticleID;
                db.SaveChanges();
            }


            // try clone new locale for this new article
            if (latestArticle != null && article != null)
            {
                tryCloningNewLocaleArticleForNewArticleVersion(latestArticle, article);
            }

            return null;
        }


        void tryCloningNewLocaleArticleForNewArticleVersion(ContentPage latestArticle, ContentPage newArticle)
        {
            // try clone new locale for this new article
            var articles = findAllLocaleArticlesByBaseArticleAndVersion(latestArticle, latestArticle.Lang);
            foreach (var _a in articles)
            {
                ContentPage _new = _a.makeNewContentPageByCloningContent();
                _new.Version = newArticle.Version;
                _new.createdBy = SessionPersister.account.AccountID;
                getArticleDb().Add(_new);
            }
            db.SaveChanges();
        }


        public String tryCreateNewLocaleArticle(ContentPage article)
        {
            if (article.BaseArticleID != 0)
            {
                if (article.Version == 0)
                {
                    var latestArticle = findLatestArticleByBaseArticle(article, article.Lang);
                    article.Version = latestArticle.Version;
                }
            }

            if (articleWithSameVersionAndLangAlreadyPresents(article))
            {
                return "ContentPage with the same version and language already presents";
            }

            article.createdBy = SessionPersister.account.AccountID;
            getArticleDb().Add(article);
            db.SaveChanges();


            if (article.BaseArticleID == 0)
            {
                db.Entry(article).State = EntityState.Modified;
                article.BaseArticleID = article.ArticleID;
                db.SaveChanges();
            }

            return null;
        }

        public String tryEditArticle(ContentPage article)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                return "Item is frozen";
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.Name = article.Name;
            _article.Desc = article.Desc;
            _article.Url = article.Url;
            _article.Keywords = article.Keywords;
            _article.Excerpt = article.Excerpt;
            db.SaveChanges();

            return null;
        }



        public String tryEditArticleProperties(ContentPage article, bool allLocales)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                return "Item is frozen";
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.Url = article.Url;
            _article.Keywords = article.Keywords;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.Url = article.Url;
                    _a.Keywords = article.Keywords;
                }
            }

            db.SaveChanges();

            return null;
        }


        public String tryDeleteArticle(ContentPage article)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }
            if (_article.isFrozen)
            {
                return "Item is frozen";
            }

            getArticleDb().Remove(article);
            db.SaveChanges();

            return null;
        }







    }
}