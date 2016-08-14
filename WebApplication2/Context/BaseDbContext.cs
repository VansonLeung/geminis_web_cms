﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Models.Infrastructure;

namespace WebApplication2.Context
{
    public class BaseDbContext : DbContext
    {
        // CMS Admin Accounts Table
        public DbSet<Account> accountDb { get; set; }

        // Articles Table + Published Articles Table
        public DbSet<Article> articleDb { get; set; }
        public DbSet<ArticlePublished> articlePublishedDb { get; set; }

        // Content Pages Table
        public DbSet<ContentPage> contentPageDb { get; set; }
        public DbSet<ContentPagePublished> contentPagePublishedDb { get; set; }

        // Infrastructures Tables
        public DbSet<Menuitem> infrastructureMenuitemDb { get; set; }
        public DbSet<Category> infrastructureCategoryDb { get; set; }




        public override int SaveChanges()
        {
            AddTimeStamps();
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw e;
            }
        }
        
        protected void AddTimeStamps()
        {

            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name)
                ? HttpContext.Current.User.Identity.Name
                : "Anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseModel)entity.Entity).created_at = DateTime.UtcNow;
                }

                ((BaseModel)entity.Entity).modified_at = DateTime.UtcNow;
            }

        }
    }
}