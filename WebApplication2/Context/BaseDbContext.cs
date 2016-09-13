using System;
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
        protected static BaseDbContext instance;
        public static BaseDbContext getInstance()
        {
            if (instance == null)
            {
                instance = new BaseDbContext();
            }
            return instance;
        }
        public static BaseDbContext newInstance()
        {
            return new BaseDbContext();
        }

        // CMS Admin Accounts Table
        public DbSet<Account> accountDb { get; set; }

        // Articles Table + Published Articles Table
        public DbSet<Article> articleDb { get; set; }
        public DbSet<ArticlePublished> articlePublishedDb { get; set; }

        // Content Pages Table
        public DbSet<ContentPage> contentPageDb { get; set; }
        public DbSet<ContentPagePublished> contentPagePublishedDb { get; set; }

        // Infrastructures Tables
        public DbSet<Category> infrastructureCategoryDb { get; set; }

        // Groups and Access Rights
        public DbSet<AccountGroup> accountGroupDb { get; set; }
        public DbSet<AccountAccessRightsContentPage> accountAccessRightsContentPageDb { get; set; }

        // System Maintenance Notifications
        public DbSet<SystemMaintenanceNotification> systemMaintenanceNotificationDb { get; set; }

        // Audit Logs
        public DbSet<AuditLog> auditLogDb { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContentPage>()
                .HasOptional(r => r.category)
                .WithMany()
                .HasForeignKey(r => r.categoryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Article>()
                .HasOptional(r => r.category)
                .WithMany()
                .HasForeignKey(r => r.categoryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AuditLog>()
                .HasOptional(r => r.account)
                .WithMany()
                .HasForeignKey(r => r.accountID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AuditLog>()
                .HasOptional(r => r.article)
                .WithMany()
                .HasForeignKey(r => r.articleID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AuditLog>()
                .HasOptional(r => r.contentPage)
                .WithMany()
                .HasForeignKey(r => r.contentPageID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AuditLog>()
                .HasOptional(r => r.category)
                .WithMany()
                .HasForeignKey(r => r.categoryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AccountAccessRightsContentPage>()
                .HasOptional(r => r.Account)
                .WithMany()
                .HasForeignKey(r => r.AccountID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BaseArticle>()
                .HasOptional(r => r.createdByAccount)
                .WithMany()
                .HasForeignKey(r => r.createdBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BaseArticle>()
                .HasOptional(r => r.approvedByAccount)
                .WithMany()
                .HasForeignKey(r => r.approvedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BaseArticle>()
                .HasOptional(r => r.publishedByAccount)
                .WithMany()
                .HasForeignKey(r => r.publishedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Category>()
                .HasOptional(r => r.parentItem)
                .WithMany()
                .HasForeignKey(r => r.parentItemID)
                .WillCascadeOnDelete(false);
        }


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