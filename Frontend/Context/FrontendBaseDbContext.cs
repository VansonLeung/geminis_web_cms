using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Frontend.Models;

namespace Frontend.Context
{
    public class FrontendBaseDbContext : DbContext
    {
        protected static FrontendBaseDbContext instance;
        public static FrontendBaseDbContext getInstance()
        {
            if (instance == null)
            {
                instance = new FrontendBaseDbContext();
            }
            return instance;
        }
        public static FrontendBaseDbContext newInstance()
        {
            return new FrontendBaseDbContext();
        }

        public DbSet<User> userDb { get; set; }
        public DbSet<Code> codeDb { get; set; }
        public DbSet<IPAddress> ipaddressDb { get; set; }

        public override int SaveChanges()
        {
            AddTimeStamps();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FrontendBaseDbContext>(null);
            base.OnModelCreating(modelBuilder);
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