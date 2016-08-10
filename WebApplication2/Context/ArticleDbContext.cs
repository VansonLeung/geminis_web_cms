﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Security;

namespace WebApplication2.Context
{
    public class ArticleDbContext
    {
        // singleton

        private static ArticleDbContext articleDbContext;

        public static ArticleDbContext getInstance()
        {
            if (articleDbContext == null)
            {
                articleDbContext = new ArticleDbContext();
            }
            return articleDbContext;
        }


        // initialization

        private BaseDbContext db = new BaseDbContext();

        protected DbSet<Article> getArticleDb()
        {
            return db.articleDb;
        }






        // methods

        public List<Article> findArticles()
        {
            return (getArticleDb()).Include(acc => acc.createdByAccount).ToList();
        }

        public List<Article> findArticlesGroupByBaseVersion(string lang = "en")
        {
            return getArticleDb()
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == lang).OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .OrderByDescending(acc => acc.modified_at)
                .Include(acc => acc.createdByAccount)
                .ToList();
        }


        public Article findArticleByID(int articleID)
        {
            return getArticleDb().Where(acc => acc.ArticleID == articleID).FirstOrDefault();
        }


        public List<Article> findArticlesRequestingApproval()
        {
            return getArticleDb().Where(acc =>
            acc.isRequestingApproval == true)
            .OrderByDescending(acc => acc.modified_at)
            .Include(acc => acc.createdByAccount)
            .ToList();
        }

        public List<Article> findArticlesGroupByBaseVersionRequestingApproval(string lang = "en")
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

        public List<Article> findArticlesGroupByBaseVersionApproved(string lang = "en")
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


        public Article findArticleByVersionAndLang(int baseArticleID, int version = 0, String lang = null)
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

        public Article findArticleByArticle(Article article)
        {
            return null;
            //   return articleDb.Where(acc => acc.Username == account.Username && acc.Password == account.Password).FirstOrDefault();
        }


        public Article findLatestArticleByBaseArticle(Article article, String lang = null)
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


        public Article findLatestArticleByBaseArticleID(int baseArticleID, String lang = null)
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



        public List<Article> findAllArticlesByBaseArticle(Article article, String lang = null)
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





        public List<Article> findAllLocaleArticlesByBaseArticleAndVersion(Article article, String Lang = "en")
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




        public bool articleWithSameVersionAndLangAlreadyPresents(Article article)
        {
            return findArticleByVersionAndLang(article.BaseArticleID, article.Version, article.Lang) != null;
        }




        // ARTICLE EDITOR ONLY



        public String tryCreateNewArticle(Article article)
        {
            Article latestArticle = null;

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
                return "Article already presents";
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


        void tryCloningNewLocaleArticleForNewArticleVersion(Article latestArticle, Article newArticle)
        {
            // try clone new locale for this new article
            var articles = findAllLocaleArticlesByBaseArticleAndVersion(latestArticle, latestArticle.Lang);
            foreach (var _a in articles)
            {
                Article _new = _a.makeNewArticleByCloningContent();
                _new.Version = newArticle.Version;
                _new.createdBy = SessionPersister.account.AccountID;
                getArticleDb().Add(_new);
            }
            db.SaveChanges();
        }


        public String tryCreateNewLocaleArticle(Article article)
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
                return "Article with the same version and language already presents";
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

        public String tryEditArticle(Article article)
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
            _article.Slug = article.Slug;
            _article.Keywords = article.Keywords;
            _article.Excerpt = article.Excerpt;
            db.SaveChanges();

            return null;
        }



        public String tryEditArticleProperties(Article article, bool allLocales)
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
            _article.Slug = article.Slug;
            _article.Keywords = article.Keywords;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.Slug = article.Slug;
                    _a.Keywords = article.Keywords;
                }
            }

            db.SaveChanges();

            return null;
        }


        public String tryDeleteArticle(Article article)
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

















        // ARTICLE EDITOR REQUEST FOR APPROVAL

        public String trySubmitRequestForApproval(Article article, bool allLocales)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.isRequestingApproval = true;
            _article.isFrozen = true;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _article.isRequestingApproval = true;
                    _article.isFrozen = true;
                }
            }

            db.SaveChanges();

            return null;
        }




        // ARTICLE APPROVER ONLY

        public String tryRequestApproval(Article article, bool allLocales)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.isApproved = true;
            _article.isFrozen = true;
            _article.dateApproved = DateTime.UtcNow;
            _article.approvedBy = SessionPersister.account.AccountID;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isApproved = true;
                    _a.isFrozen = true;
                    _a.dateApproved = DateTime.UtcNow;
                    _a.approvedBy = SessionPersister.account.AccountID;
                }
            }

            db.SaveChanges();

            return null;
        }

        public String tryRequestUnapproval(Article article, bool allLocales)
        {
            var _article = findArticleByID(article.ArticleID);
            if (_article == null)
            {
                return "Item not found";
            }

            db.Entry(_article).State = EntityState.Modified;
            _article.isApproved = false;
            _article.isFrozen = true;
            _article.dateApproved = null;
            _article.approvedBy = SessionPersister.account.AccountID;

            if (allLocales)
            {
                var _localeArticles = findAllLocaleArticlesByBaseArticleAndVersion(article, article.Lang);
                foreach (var _a in _localeArticles)
                {
                    db.Entry(_a).State = EntityState.Modified;
                    _a.isApproved = false;
                    _a.isFrozen = true;
                    _a.dateApproved = null;
                    _a.approvedBy = SessionPersister.account.AccountID;
                }
            }

            db.SaveChanges();

            return null;
        }


    }
}