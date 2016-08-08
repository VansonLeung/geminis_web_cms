using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Context
{
    public class ArticleDbContext : BaseDbContext
    {
        public DbSet<Article> articleDb { get; set; }

        public List<Article> findArticles()
        {
            return articleDb.ToList();
        }

        public List<Article> findArticlesGroupByBaseVersion()
        {
            return articleDb
                .GroupBy(acc => acc.BaseArticleID)
                .Select(u => u.Where(acc => acc.Lang == "en").OrderByDescending(acc => acc.Version)
                .FirstOrDefault())
                .OrderByDescending(acc => acc.modified_at)
                .ToList();
        }


        public Article findArticleByID(int articleID)
        {
            return articleDb.Where(acc => acc.ArticleID == articleID).FirstOrDefault();
        }


        public List<Article> findArticlesRequestingApproval()
        {
            return articleDb.Where(acc => 
            acc.isRequestingApproval == true)
            .OrderByDescending(acc => acc.modified_at)
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
                return articleDb.Where(acc =>
                acc.BaseArticleID == baseArticleID &&
                acc.Lang == lang)
                .OrderByDescending(acc => acc.Version)
                .FirstOrDefault();
            }
            else
            {
                return articleDb.Where(acc =>
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
            
            var baseLatestArticle = articleDb.Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
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

            var baseLatestArticle = articleDb.Where(acc =>
            acc.BaseArticleID == baseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .FirstOrDefault();


            if (baseLatestArticle != null)
            {
                return baseLatestArticle;
            }

            return null;
        }



        public List<Article> findAllArticlesByBaseArticle(Article article, String lang = null)
        {
            var articles = articleDb.Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang == (lang != null ? lang : "en")
            )
            .OrderByDescending(acc => acc.Version)
            .ToList();

            return articles;
        }





        public List<Article> findAllLocaleArticlesByBaseArticleAndVersion(Article article)
        {
            var articles = articleDb.Where(acc =>
            acc.BaseArticleID == article.BaseArticleID &&
            acc.Lang != "en" &&
            acc.Version == article.Version
            )
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

            articleDb.Add(article);
            SaveChanges();
            

            if (article.BaseArticleID == 0)
            {
                Entry(article).State = EntityState.Modified;
                article.BaseArticleID = article.ArticleID;
                SaveChanges();
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
            var articles = findAllLocaleArticlesByBaseArticleAndVersion(latestArticle);
            foreach (var _a in articles)
            {
                Article _new = _a.makeNewArticleByCloningContent();
                _new.Version = newArticle.Version;
                articleDb.Add(_new);
            }
            SaveChanges();
        }


        public String tryCreateNewLocaleArticle(Article article)
        {
            if (article.BaseArticleID != 0)
            {
                var latestArticle = findLatestArticleByBaseArticle(article, "en");
                article.Version = latestArticle.Version;
            }

            if (articleWithSameVersionAndLangAlreadyPresents(article))
            {
                return "Article already presents";
            }

            articleDb.Add(article);
            SaveChanges();


            if (article.BaseArticleID == 0)
            {
                Entry(article).State = EntityState.Modified;
                article.BaseArticleID = article.ArticleID;
                SaveChanges();
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

            Entry(_article).State = EntityState.Modified;
            _article.Name = article.Name;
            _article.Desc = article.Desc;
            _article.Slug = article.Slug;
            _article.Keywords = article.Keywords;
            _article.Excerpt = article.Excerpt;
            SaveChanges();

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

            articleDb.Remove(article);
            SaveChanges();

            return null;
        }
























        // ARTICLE APPROVER ONLY

        public String tryRequestApproval(Article article)
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

            Entry(_article).State = EntityState.Modified;
            _article.isApproved = true;
            _article.dateApproved = DateTime.UtcNow;
            SaveChanges();

            return null;
        }

        public String tryRequestUnapproval(Article article)
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

            Entry(_article).State = EntityState.Modified;
            _article.isApproved = false;
            _article.dateApproved = null;
            SaveChanges();

            return null;
        }





















    }
}